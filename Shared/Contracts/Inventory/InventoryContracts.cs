using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Inventory;

/// <summary>
/// Base request to create a new product in inventory.
/// Web endpoint derives from this to add [From(Claim.X)] and [FromClaim] attributes.
/// </summary>
public class CreateProductRequestBase
{
    /// <summary>
    /// User ID bound from claims in Web
    /// </summary>
    public virtual string? UserID { get; set; }
    
    /// <summary>
    /// Username bound from claims in Web
    /// </summary>
    public virtual string? Username { get; set; }
    
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Description { get; set; }
    public virtual decimal Price { get; set; }
    public virtual int QtyOnHand { get; set; }
    public virtual string? ModifiedBy { get; set; }
    public virtual bool GenerateFullUrl { get; set; }
}

/// <summary>
/// Client-facing request to create a product.
/// </summary>
public sealed class CreateProductRequest : CreateProductRequestBase, IApiRequest<CreateProductRequest, CreateProductResponse>
{
    public static string Route => ApiRoutes.Inventory_Create;
    public static HttpMethod Method => HttpMethod.Post;
    public static JsonTypeInfo<CreateProductRequest> RequestTypeInfo => SharedJsonContext.Default.CreateProductRequest;
    public static JsonTypeInfo<CreateProductResponse> ResponseTypeInfo => SharedJsonContext.Default.CreateProductResponse;
}

/// <summary>
/// Base request to update a product in inventory.
/// Web endpoint derives from this to add [From(Claim.X)] attribute.
/// </summary>
public class UpdateProductRequestBase
{
    /// <summary>
    /// User ID bound from claims in Web
    /// </summary>
    public virtual string? UserID { get; set; }
    
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Description { get; set; }
    public virtual decimal Price { get; set; }
    public virtual int QtyOnHand { get; set; }
    public virtual string? ModifiedBy { get; set; }
}

/// <summary>
/// Client-facing request to update a product.
/// </summary>
public sealed class UpdateProductRequest : UpdateProductRequestBase, IApiRequest<UpdateProductRequest, UpdateProductResponse>
{
    public static string Route => ApiRoutes.Inventory_Update;
    public static HttpMethod Method => HttpMethod.Put;
    public static JsonTypeInfo<UpdateProductRequest> RequestTypeInfo => SharedJsonContext.Default.UpdateProductRequest;
    public static JsonTypeInfo<UpdateProductResponse> ResponseTypeInfo => SharedJsonContext.Default.UpdateProductResponse;
}

/// <summary>
/// Response from creating a product.
/// </summary>
public sealed class CreateProductResponse
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}

/// <summary>
/// Response from updating a product.
/// </summary>
public sealed class UpdateProductResponse
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}

/// <summary>
/// Base request to delete a product.
/// Web endpoint may add [DefaultValue] attribute.
/// </summary>
public class DeleteProductRequestBase
{
    public virtual string ItemID { get; set; } = "";
}

/// <summary>
/// Client-facing request to delete a product.
/// </summary>
public sealed class DeleteProductRequest : DeleteProductRequestBase, IApiRequest<DeleteProductRequest, Customers.EmptyResponse>
{
    public static string Route => ApiRoutes.Inventory_Delete;
    public static HttpMethod Method => HttpMethod.Delete;
    public static JsonTypeInfo<DeleteProductRequest> RequestTypeInfo => SharedJsonContext.Default.DeleteProductRequest;
    public static JsonTypeInfo<Customers.EmptyResponse> ResponseTypeInfo => SharedJsonContext.Default.EmptyResponse;
}

/// <summary>
/// Client-facing request to get a product.
/// </summary>
public sealed class GetProductRequest : IApiRequest<GetProductRequest, GetProductResponse>
{
    public string ProductID { get; set; } = "";
    
    public static string Route => ApiRoutes.Inventory_GetProduct;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<GetProductRequest> RequestTypeInfo => SharedJsonContext.Default.GetProductRequest;
    public static JsonTypeInfo<GetProductResponse> ResponseTypeInfo => SharedJsonContext.Default.GetProductResponse;
}

/// <summary>
/// Response from getting a product.
/// </summary>
public sealed class GetProductResponse
{
    public string? ProductID { get; set; }

    [JsonPropertyName("Last_Moddded")]
    public long LastModified { get; set; }
}

/// <summary>
/// Client-facing request to list recent inventory.
/// </summary>
public sealed class ListRecentInventoryRequest : IApiRequest<ListRecentInventoryRequest, ListRecentInventoryResponse>
{
    public string CategoryID { get; set; } = "";
    
    public static string Route => ApiRoutes.Inventory_ListRecent;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<ListRecentInventoryRequest> RequestTypeInfo => SharedJsonContext.Default.ListRecentInventoryRequest;
    public static JsonTypeInfo<ListRecentInventoryResponse> ResponseTypeInfo => SharedJsonContext.Default.ListRecentInventoryResponse;
}

/// <summary>
/// Response listing recent inventory items.
/// </summary>
public sealed class ListRecentInventoryResponse
{
    public string? Category { get; set; }
}
