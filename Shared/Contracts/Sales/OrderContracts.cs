using FastEndpoints;
using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Sales.Orders;

/// <summary>
/// Request to create a new order.
/// Implements IApiRequest for type-safe HTTP calls.
/// </summary>
/// <example>
/// var (response, result) = await client.SendAsJsonAsync(new CreateOrderRequest
/// {
///     CustomerID = 123,
///     ProductID = 456,
///     Quantity = 2
/// });
/// </example>
public sealed class CreateOrderRequest : IApiRequest<CreateOrderRequest, CreateOrderResponse>
{
    public int CustomerID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }

    /// <summary>
    /// GUID for testing/tracking
    /// </summary>
    public Guid GuidTest { get; set; }

    // IApiRequest implementation
    public static string Route => ApiRoutes.Sales_Orders_Create;
    public static HttpMethod Method => HttpMethod.Post;
    public static JsonTypeInfo<CreateOrderRequest> RequestTypeInfo => SharedJsonContext.Default.CreateOrderRequest;
    public static JsonTypeInfo<CreateOrderResponse> ResponseTypeInfo => SharedJsonContext.Default.CreateOrderResponse;
}

/// <summary>
/// Response from creating an order.
/// </summary>
public sealed class CreateOrderResponse
{
    public int OrderID { get; set; }
    public string? Message { get; set; }
    public string? AnotherMsg { get; set; }
    public Guid GuidTest { get; set; }
    public int Header1 { get; set; }
    public DateOnly Header2 { get; set; }
}

/// <summary>
/// Request to retrieve an order.
/// Implements IApiRequest for type-safe HTTP calls.
/// </summary>
/// <example>
/// var (response, result) = await client.SendAsJsonAsync(new RetrieveOrderRequest
/// {
///     OrderID = "ORD-001"
/// });
/// </example>
public sealed class RetrieveOrderRequest : IApiRequest<RetrieveOrderRequest, RetrieveOrderResponse>
{
    /// <summary>
    /// The tenant id
    /// </summary>
    [FromHeader("tenant-id", IsRequired = false)]
    public string TenantID { get; set; } = "";

    public string OrderID { get; set; } = "";

    [FromHeader("Content-Type")]
    public string ContentType { get; set; } = "";

    // IApiRequest implementation
    public static string Route => ApiRoutes.Sales_Orders_Retrieve;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<RetrieveOrderRequest> RequestTypeInfo => SharedJsonContext.Default.RetrieveOrderRequest;
    public static JsonTypeInfo<RetrieveOrderResponse> ResponseTypeInfo => SharedJsonContext.Default.RetrieveOrderResponse;
}

/// <summary>
/// Response from retrieving an order.
/// </summary>
public sealed class RetrieveOrderResponse
{
    public string Message => "ok!";
}
