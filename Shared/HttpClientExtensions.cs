using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

namespace Shared;

/// <summary>
/// Result of an API call containing both the HTTP response and the deserialized result.
/// Supports deconstruction: var (response, result) = await request.SendAsync(client);
/// </summary>
public readonly record struct ApiResult<TResponse>(HttpResponseMessage Response, TResponse? Result);

/// <summary>
/// Marker interface that enables C# type inference for the response type.
/// This is a simpler interface that only exposes TResponse, allowing extension methods
/// to infer TResponse from the request object.
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IReturn<TResponse>
{
    /// <summary>
    /// Sends this request using the specified HttpClient.
    /// Implementations should use static metadata for AOT-compatibility.
    /// </summary>
    Task<ApiResult<TResponse>> SendAsync(HttpClient client, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for API requests that carry their own metadata for AOT-compatible HTTP calls.
/// The request type knows its route, HTTP method, and how to serialize/deserialize.
/// Route parameters like {Id} are automatically substituted from matching properties.
/// </summary>
/// <typeparam name="TSelf">The request type itself (CRTP pattern)</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IApiRequest<TSelf, TResponse> : IReturn<TResponse>
    where TSelf : IApiRequest<TSelf, TResponse>
{
    /// <summary>The API route (e.g., "/api/admin/login" or "/api/items/{Id}")</summary>
    static abstract string Route { get; }
    
    /// <summary>The HTTP method (GET, POST, PUT, DELETE, PATCH)</summary>
    static abstract HttpMethod Method { get; }
    
    /// <summary>AOT-compatible JSON serialization metadata for the request type</summary>
    static abstract JsonTypeInfo<TSelf> RequestTypeInfo { get; }
    
    /// <summary>AOT-compatible JSON serialization metadata for the response type</summary>
    static abstract JsonTypeInfo<TResponse> ResponseTypeInfo { get; }

    /// <summary>
    /// Sends this request using the specified HttpClient.
    /// Default implementation uses the static metadata from the interface.
    /// Route parameters like {PropertyName} are automatically hydrated from the request's properties.
    /// </summary>
    async Task<ApiResult<TResponse>> IReturn<TResponse>.SendAsync(HttpClient client, CancellationToken cancellationToken)
    {
        var method = TSelf.Method;
        var route = RouteHelper.HydrateRoute(TSelf.Route, (TSelf)this, TSelf.RequestTypeInfo);
        
        HttpResponseMessage response;
        
        if (method == HttpMethod.Get)
        {
            response = await client.GetAsync(route, cancellationToken);
        }
        else if (method == HttpMethod.Delete)
        {
            response = await client.DeleteAsync(route, cancellationToken);
        }
        else
        {
            var content = JsonContent.Create((TSelf)this, TSelf.RequestTypeInfo);
            
            if (method == HttpMethod.Post)
            {
                response = await client.PostAsync(route, content, cancellationToken);
            }
            else if (method == HttpMethod.Put)
            {
                response = await client.PutAsync(route, content, cancellationToken);
            }
            else if (method == HttpMethod.Patch)
            {
                response = await client.PatchAsync(route, content, cancellationToken);
            }
            else
            {
                var httpRequest = new HttpRequestMessage(method, route) { Content = content };
                response = await client.SendAsync(httpRequest, cancellationToken);
            }
        }

        TResponse? result = default;
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync(TSelf.ResponseTypeInfo, cancellationToken);
        }

        return new ApiResult<TResponse>(response, result);
    }
}

/// <summary>
/// Helper for hydrating route parameters from request properties.
/// AOT-compatible using JsonTypeInfo metadata.
/// </summary>
internal static partial class RouteHelper
{
    // Source-generated regex for route parameters (AOT-compatible)
    [GeneratedRegex(@"\{(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex RouteParameterRegex();

    /// <summary>
    /// Replaces route parameters like {PropertyName} with values from the request object.
    /// Uses JsonTypeInfo for AOT-compatible property access.
    /// </summary>
    internal static string HydrateRoute<T>(string routeTemplate, T request, JsonTypeInfo<T> typeInfo)
    {
        if (!routeTemplate.Contains('{'))
            return routeTemplate;

        return RouteParameterRegex().Replace(routeTemplate, match =>
        {
            var paramName = match.Groups[1].Value;
            
            // Find property in JsonTypeInfo (AOT-compatible)
            foreach (var prop in typeInfo.Properties)
            {
                if (string.Equals(prop.Name, paramName, StringComparison.OrdinalIgnoreCase))
                {
                    var value = prop.Get?.Invoke(request!);
                    return Uri.EscapeDataString(value?.ToString() ?? "");
                }
            }
            
            // Property not found - leave placeholder (will likely cause 404)
            return match.Value;
        });
    }
}

/// <summary>
/// Extension methods for HttpClient that enable clean, AOT-compatible API calls.
/// </summary>
/// <example>
/// // Clean syntax - just pass the request, response type is inferred from IReturn&lt;TResponse&gt;:
/// var (response, result) = await client.SendAsync(new LoginRequest { Email = "user@example.com", Password = "pass" });
/// </example>
public static class HttpClientApiExtensions
{
    /// <summary>
    /// Sends an API request. The request knows its own route, method, and serialization.
    /// TResponse is inferred from the IReturn&lt;TResponse&gt; marker interface.
    /// </summary>
    public static Task<ApiResult<TResponse>> SendAsync<TResponse>(
        this HttpClient client,
        IReturn<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        return request.SendAsync(client, cancellationToken);
    }
    
    /// <summary>
    /// POST JSON and deserialize the response using explicit type info.
    /// Use this when you need to specify the serialization context explicitly.
    /// </summary>
    public static async Task<(HttpResponseMessage Response, TResponse? Result)> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string requestUri,
        TRequest request,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(requestUri, request, requestTypeInfo, cancellationToken);
        var result = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken)
            : default;
        return (response, result);
    }

    /// <summary>
    /// GET and deserialize the response using explicit type info.
    /// Use this when you need to specify the serialization context explicitly.
    /// </summary>
    public static async Task<(HttpResponseMessage Response, TResponse? Result)> GetAsJsonAsync<TResponse>(
        this HttpClient client,
        string requestUri,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(requestUri, cancellationToken);
        var result = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken)
            : default;
        return (response, result);
    }

    /// <summary>
    /// PUT JSON and deserialize the response using explicit type info.
    /// </summary>
    public static async Task<(HttpResponseMessage Response, TResponse? Result)> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string requestUri,
        TRequest request,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(requestUri, request, requestTypeInfo, cancellationToken);
        var result = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken)
            : default;
        return (response, result);
    }

    /// <summary>
    /// DELETE and deserialize the response using explicit type info.
    /// </summary>
    public static async Task<(HttpResponseMessage Response, TResponse? Result)> DeleteAsJsonAsync<TResponse>(
        this HttpClient client,
        string requestUri,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(requestUri, cancellationToken);
        var result = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken)
            : default;
        return (response, result);
    }
}
