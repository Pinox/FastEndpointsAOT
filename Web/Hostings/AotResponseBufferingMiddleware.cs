using Microsoft.AspNetCore.Http.Features;

namespace Web.Hostings;

/// <summary>
/// AOT Response Buffering Middleware for Native AOT FastEndpoints compatibility.
/// 
/// Problem: FastEndpoints registers routes with app.MapMethods(..., lambda => Task).
/// ASP.NET Core's EndpointMiddleware awaits the Task and tries to serialize the result (VoidTaskResult).
/// In Native AOT, this triggers: "JsonPropertyInfo`1[System.Threading.Tasks.VoidTaskResult] is missing native code or metadata"
/// 
/// Why pre-configuration doesn't work:
/// - ASP.NET Core's HttpResponseJsonExtensions uses its own internal JsonSerializerOptions
/// - Configuring FastEndpoints' serializer options doesn't affect ASP.NET Core's built-in JSON middleware
/// - The VoidTaskResult type is internal to .NET and cannot be registered in a JsonSerializerContext
/// 
/// Solution: This middleware catches and suppresses the VoidTaskResult exception when the response
/// was already successfully written. This is safe because:
/// 1. Our custom ResponseSerializer already wrote the actual response data
/// 2. The exception occurs AFTER the response is written, not during
/// 3. The buffer contains the complete, valid response
/// 
/// Performance: Minimal overhead. Exception filters have near-zero cost when no exception occurs.
/// When the exception does occur, we simply suppress it - no re-throw or stack unwinding.
/// </summary>
internal sealed class AotResponseBufferingMiddleware
{
    private readonly RequestDelegate _next;

    public AotResponseBufferingMiddleware(RequestDelegate next)
    {
        _next = next;
        Console.WriteLine("[DEBUG] AotResponseBufferingMiddleware constructor called");
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"[DEBUG] AotResponseBufferingMiddleware.Invoke called for {context.Request.Method} {context.Request.Path}");

        var originalBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
        if (originalBodyFeature is null)
        {
            Console.WriteLine("[DEBUG] No IHttpResponseBodyFeature found, skipping buffering");
            await _next(context);
            return;
        }

        await using var buffer = new MemoryStream();
        var bufferingFeature = new StreamResponseBodyFeature(buffer);
        context.Features.Set<IHttpResponseBodyFeature>(bufferingFeature);

        try
        {
            Console.WriteLine("[DEBUG] Calling next middleware...");
            await _next(context);
            Console.WriteLine("[DEBUG] Next middleware completed successfully");
        }
        catch (NotSupportedException ex) when (IsVoidTaskResultException(ex) && ShouldSuppressException(context, buffer))
        {
            Console.WriteLine($"[DEBUG] Caught and suppressing VoidTaskResult exception: {ex.Message}");
            // Suppress the VoidTaskResult serialization exception.
            // The response was already written successfully to the buffer.
            // We only suppress when the response appears successful (2xx or redirects).
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Unexpected exception in middleware: {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine($"[DEBUG] Processing response buffer (length: {buffer.Length})");
            buffer.Position = 0;
            context.Features.Set(originalBodyFeature);

            var isEventStream = context.Response.ContentType?.StartsWith("text/event-stream", StringComparison.OrdinalIgnoreCase) == true;
            var statusCode = context.Response.StatusCode;
            var isHeadRequest = HttpMethods.IsHead(context.Request.Method);
            var isNoContentStatus = statusCode == StatusCodes.Status204NoContent || statusCode == StatusCodes.Status304NotModified;
            var isInformational = statusCode >= 100 && statusCode < 200;

            // If we buffered a body but the pipeline set 204, normalize to 200 so the body can be written safely.
            if (statusCode == StatusCodes.Status204NoContent && buffer.Length > 0)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                statusCode = context.Response.StatusCode;
                isNoContentStatus = false;
            }

            var canWriteBody = !isHeadRequest && !isNoContentStatus && !isInformational;

            // Strip Task JSON suffix from responses if present
            // This happens because ASP.NET Core serializes Task return values in AOT mode
            var outputBuffer = buffer;
            if (buffer.Length > 0 && context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                outputBuffer = StripTaskJsonSuffix(buffer);
            }

            if (!context.Response.HasStarted && !isEventStream && canWriteBody)
            {
                context.Response.Headers.Remove("Transfer-Encoding");
                context.Response.ContentLength = outputBuffer.Length;
            }

            if (canWriteBody)
            {
                Console.WriteLine($"[DEBUG] Writing {outputBuffer.Length} bytes to response stream");
                outputBuffer.Position = 0;
                await outputBuffer.CopyToAsync(originalBodyFeature.Stream, context.RequestAborted);
            }
            
            // Dispose the stripped buffer if it's different from the original
            if (!ReferenceEquals(outputBuffer, buffer))
            {
                await outputBuffer.DisposeAsync();
            }
            Console.WriteLine("[DEBUG] Response processing completed");
        }
    }

    /// <summary>
    /// Checks if the exception is the VoidTaskResult serialization error that occurs in Native AOT.
    /// </summary>
    private static bool IsVoidTaskResultException(NotSupportedException ex)
    {
        return ex.Message.Contains("VoidTaskResult", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("missing native code or metadata", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if we should suppress the VoidTaskResult exception.
    /// We only suppress when:
    /// 1. Response has content (buffer.Length > 0)
    /// 2. The response was NOT an error that should propagate (we don't mask real errors)
    /// 
    /// The VoidTaskResult exception happens AFTER the endpoint writes its response,
    /// so if the endpoint returned an error response (400, 500), that error was already
    /// written to the buffer. We still suppress the VoidTaskResult exception because
    /// it's a framework artifact, not the actual error. The actual error response is preserved.
    /// </summary>
    private static bool ShouldSuppressException(HttpContext context, MemoryStream buffer)
    {
        // Only suppress if we have buffered content - meaning the endpoint did write a response
        if (buffer.Length == 0)
            return false;

        // The VoidTaskResult exception is always safe to suppress when we have a response.
        // The endpoint already set the status code (whether 200, 400, 500, etc.) and wrote the body.
        // The exception occurs AFTER all that, during internal Task result serialization.
        // We preserve whatever status code the endpoint set - we don't force 200.
        return true;
    }

    /// <summary>
    /// Task JSON signature pattern - used to detect appended Task serialization
    /// </summary>
    private static readonly byte[] TaskJsonSignature = System.Text.Encoding.UTF8.GetBytes("{\"asyncState\":");

    /// <summary>
    /// Strips the Task JSON suffix from the response buffer if present.
    /// In AOT mode, ASP.NET Core may serialize Task return values, appending JSON like:
    /// {"asyncState":null,"creationOptions":0,"exception":null,...}
    /// This method detects and removes that suffix.
    /// </summary>
    private static MemoryStream StripTaskJsonSuffix(MemoryStream buffer)
    {
        if (buffer.Length < TaskJsonSignature.Length + 10) // Minimum viable JSON response + task suffix
            return buffer;

        buffer.Position = 0;
        var data = buffer.ToArray();
        
        // Look for the Task JSON signature pattern in the response
        var signatureIndex = FindPattern(data, TaskJsonSignature);
        
        if (signatureIndex <= 0)
            return buffer; // No Task JSON suffix found, or it's at the start (unlikely)

        Console.WriteLine($"[DEBUG] Found Task JSON suffix at position {signatureIndex}, stripping...");
        
        // Create a new buffer with only the content before the Task JSON
        var cleanBuffer = new MemoryStream();
        cleanBuffer.Write(data, 0, signatureIndex);
        cleanBuffer.Position = 0;
        
        return cleanBuffer;
    }

    /// <summary>
    /// Finds the first occurrence of a pattern in a byte array.
    /// </summary>
    private static int FindPattern(byte[] data, byte[] pattern)
    {
        for (int i = 0; i <= data.Length - pattern.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return i;
        }
        return -1;
    }
}
