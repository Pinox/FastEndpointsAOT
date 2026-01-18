using FastEndpoints;
using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Customers;

/// <summary>
/// Base request to create a new customer.
/// Web endpoint derives from this to add [From(Claim.X)] and [HasPermission(Allow.Y)] attributes.
/// </summary>
public class CreateCustomerRequestBase
{
    public virtual string CustomerID { get; set; } = "";
    public virtual string? CreatedBy { get; set; }
    
    /// <summary>
    /// The name of the customer
    /// </summary>
    public virtual string? CustomerName { get; set; }
    public virtual IEnumerable<string> PhoneNumbers { get; set; } = [];
    
    /// <summary>
    /// Indicates if user has create permission (bound via [HasPermission] in Web)
    /// </summary>
    public virtual bool HasCreatePermission { get; set; }
}

/// <summary>
/// Client-facing request to create a customer.
/// </summary>
public sealed class CreateCustomerRequest : CreateCustomerRequestBase, IApiRequest<CreateCustomerRequest, EmptyResponse>
{
    public static string Route => ApiRoutes.Customer_Create;
    public static HttpMethod Method => HttpMethod.Post;
    public static JsonTypeInfo<CreateCustomerRequest> RequestTypeInfo => SharedJsonContext.Default.CreateCustomerRequest;
    public static JsonTypeInfo<EmptyResponse> ResponseTypeInfo => SharedJsonContext.Default.EmptyResponse;
}

/// <summary>
/// Base request to update a customer.
/// Web endpoint derives from this to add [FromClaim] and [QueryParam] bindings.
/// </summary>
public class UpdateCustomerRequestBase
{
    public virtual string CustomerID { get; set; } = "";
    public virtual string Name { get; set; } = "";
    public virtual int Age { get; set; }
    public virtual string Address { get; set; } = "";
}

/// <summary>
/// Client-facing request to update a customer.
/// </summary>
public sealed class UpdateCustomerRequest : UpdateCustomerRequestBase, IApiRequest<UpdateCustomerRequest, EmptyResponse>
{
    public static string Route => ApiRoutes.Customer_Update;
    public static HttpMethod Method => HttpMethod.Put;
    public static JsonTypeInfo<UpdateCustomerRequest> RequestTypeInfo => SharedJsonContext.Default.UpdateCustomerRequest;
    public static JsonTypeInfo<EmptyResponse> ResponseTypeInfo => SharedJsonContext.Default.EmptyResponse;
}

/// <summary>
/// Request to update a customer with header-bound values.
/// </summary>
public sealed record UpdateCustomerWithHeaderRequest(
    [property: FromHeader] int CustomerID,
    [property: FromHeader("tenant-id")] string TenantID,
    string Name,
    int Age,
    string Address);

/// <summary>
/// Client-facing request to list recent customers.
/// </summary>
public sealed class ListRecentCustomersRequest : IApiRequest<ListRecentCustomersRequest, ListRecentCustomersResponse>
{
    public static string Route => ApiRoutes.Customer_ListRecent;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<ListRecentCustomersRequest> RequestTypeInfo => SharedJsonContext.Default.ListRecentCustomersRequest;
    public static JsonTypeInfo<ListRecentCustomersResponse> ResponseTypeInfo => SharedJsonContext.Default.ListRecentCustomersResponse;
}

/// <summary>
/// Response for listing recent customers.
/// </summary>
public sealed class ListRecentCustomersResponse
{
    public IEnumerable<KeyValuePair<string, int>>? Customers { get; set; }
}

/// <summary>
/// Empty response placeholder for endpoints that don't return data.
/// </summary>
public sealed class EmptyResponse { }
