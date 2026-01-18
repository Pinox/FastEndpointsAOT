using Microsoft.AspNetCore.Http.Features;

namespace FastEndpointsNativeAOT;

/// <summary>
/// AOT Response Buffering Middleware for Native AOT FastEndpoints compatibility.
/// Catches VoidTaskResult serialization exceptions that occur in Native AOT.
/// </summary>
internal sealed class AotResponseBufferingMiddleware
{
    private readonly RequestDelegate _next;

    public AotResponseBufferingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var originalBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
        if (originalBodyFeature is null)
        {
            await _next(context);
            return;
        }

        await using var buffer = new MemoryStream();
        var bufferingFeature = new StreamResponseBodyFeature(buffer);
        context.Features.Set<IHttpResponseBodyFeature>(bufferingFeature);

        try
        {
            await _next(context);
        }
        catch (NotSupportedException ex) when (IsVoidTaskResultException(ex) && buffer.Length > 0)
        {
            // Suppress the VoidTaskResult serialization exception.
            // The response was already written successfully to the buffer.
        }
        finally
        {
            buffer.Position = 0;
            context.Features.Set(originalBodyFeature);

            var statusCode = context.Response.StatusCode;
            var isHeadRequest = HttpMethods.IsHead(context.Request.Method);
            var isNoContentStatus = statusCode == StatusCodes.Status204NoContent || statusCode == StatusCodes.Status304NotModified;

            // If we buffered a body but the pipeline set 204, normalize to 200
            if (statusCode == StatusCodes.Status204NoContent && buffer.Length > 0)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                isNoContentStatus = false;
            }

            var canWriteBody = !isHeadRequest && !isNoContentStatus;

            // Strip Task JSON suffix from responses if present
            var outputBuffer = buffer;
            if (buffer.Length > 0 && context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                outputBuffer = StripTaskJsonSuffix(buffer);
            }

            if (!context.Response.HasStarted && canWriteBody)
            {
                context.Response.Headers.Remove("Transfer-Encoding");
                context.Response.ContentLength = outputBuffer.Length;
            }

            if (canWriteBody)
            {
                outputBuffer.Position = 0;
                await outputBuffer.CopyToAsync(originalBodyFeature.Stream, context.RequestAborted);
            }

            if (!ReferenceEquals(outputBuffer, buffer))
            {
                await outputBuffer.DisposeAsync();
            }
        }
    }

    private static bool IsVoidTaskResultException(NotSupportedException ex)
    {
        return ex.Message.Contains("VoidTaskResult", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("missing native code or metadata", StringComparison.OrdinalIgnoreCase);
    }

    private static readonly byte[] TaskJsonSignature = System.Text.Encoding.UTF8.GetBytes("{\"asyncState\":");

    private static MemoryStream StripTaskJsonSuffix(MemoryStream buffer)
    {
        if (buffer.Length < TaskJsonSignature.Length + 10)
            return buffer;

        buffer.Position = 0;
        var data = buffer.ToArray();
        
        var signatureIndex = FindPattern(data, TaskJsonSignature);
        
        if (signatureIndex <= 0)
            return buffer;

        var cleanBuffer = new MemoryStream();
        cleanBuffer.Write(data, 0, signatureIndex);
        cleanBuffer.Position = 0;
        
        return cleanBuffer;
    }

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
