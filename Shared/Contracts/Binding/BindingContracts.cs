using FastEndpoints;
using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Binding;

/// <summary>
/// Request for route and query parameter binding sample.
/// </summary>
public sealed class RouteAndQueryRequest : IApiRequest<RouteAndQueryRequest, Customers.EmptyResponse>
{
    [QueryParam]
    public int Page { get; set; }

    [QueryParam]
    public int PageSize { get; set; }

    public string Id { get; set; } = "";
    
    public static string Route => ApiRoutes.Binding_RouteAndQuery;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<RouteAndQueryRequest> RequestTypeInfo => SharedJsonContext.Default.RouteAndQueryRequest;
    public static JsonTypeInfo<Customers.EmptyResponse> ResponseTypeInfo => SharedJsonContext.Default.EmptyResponse;
}
