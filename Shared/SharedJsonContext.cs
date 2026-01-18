using System.Text.Json.Serialization;
using Shared.Contracts.Admin;
using Shared.Contracts.Binding;
using Shared.Contracts.Customers;
using Shared.Contracts.Inventory;
using Shared.Contracts.Pipeline;
using Shared.Contracts.Sales.Orders;
using Shared.Contracts.Security;
using Shared.Contracts.Uploads;
using Shared.Contracts.Versioning;

namespace Shared;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata, 
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// Admin - Base type for inheritance pattern + client requests
[JsonSerializable(typeof(LoginRequestBase))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
// Binding
[JsonSerializable(typeof(RouteAndQueryRequest))]
// Customers - Base types + client requests
[JsonSerializable(typeof(CreateCustomerRequestBase))]
[JsonSerializable(typeof(CreateCustomerRequest))]
[JsonSerializable(typeof(UpdateCustomerRequestBase))]
[JsonSerializable(typeof(UpdateCustomerRequest))]
[JsonSerializable(typeof(UpdateCustomerWithHeaderRequest))]
[JsonSerializable(typeof(ListRecentCustomersRequest))]
[JsonSerializable(typeof(ListRecentCustomersResponse))]
[JsonSerializable(typeof(EmptyResponse))]
// Inventory - Base types + client requests
[JsonSerializable(typeof(CreateProductRequestBase))]
[JsonSerializable(typeof(CreateProductRequest))]
[JsonSerializable(typeof(CreateProductResponse))]
[JsonSerializable(typeof(UpdateProductRequestBase))]
[JsonSerializable(typeof(UpdateProductRequest))]
[JsonSerializable(typeof(UpdateProductResponse))]
[JsonSerializable(typeof(DeleteProductRequestBase))]
[JsonSerializable(typeof(DeleteProductRequest))]
[JsonSerializable(typeof(GetProductRequest))]
[JsonSerializable(typeof(GetProductResponse))]
[JsonSerializable(typeof(ListRecentInventoryRequest))]
[JsonSerializable(typeof(ListRecentInventoryResponse))]
// Orders
[JsonSerializable(typeof(CreateOrderRequest))]
[JsonSerializable(typeof(CreateOrderResponse))]
[JsonSerializable(typeof(RetrieveOrderRequest))]
[JsonSerializable(typeof(RetrieveOrderResponse))]
// Pipeline
[JsonSerializable(typeof(PipelineRequest))]
// Security
[JsonSerializable(typeof(VerifyClaimRequest))]
[JsonSerializable(typeof(VerifyClaimResponse))]
[JsonSerializable(typeof(WhoAmIRequest))]
[JsonSerializable(typeof(WhoAmIResponse))]
[JsonSerializable(typeof(ClaimKV))]
[JsonSerializable(typeof(ClaimKV[]))]
// Uploads - Base types for inheritance pattern
[JsonSerializable(typeof(ImageSaveRequestBase))]
[JsonSerializable(typeof(ImageSaveTypedRequestBase))]
[JsonSerializable(typeof(MultipartUploadRequestBase))]
// Versioning
[JsonSerializable(typeof(VersionSampleRequest))]
public sealed partial class SharedJsonContext : JsonSerializerContext { }
