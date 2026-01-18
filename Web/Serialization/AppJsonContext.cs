using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdminLogin = Admin.Login;
using SharedAdmin = Shared.Contracts.Admin;
using SharedSecurity = Shared.Contracts.Security;
using SharedInventory = Shared.Contracts.Inventory;
using SharedSalesOrders = Shared.Contracts.Sales.Orders;
using SharedCustomers = Shared.Contracts.Customers;
using SharedBinding = Shared.Contracts.Binding;
using SharedPipeline = Shared.Contracts.Pipeline;
using SharedVersioning = Shared.Contracts.Versioning;
using SharedUploads = Shared.Contracts.Uploads;
using SecurityClaims = Security.Claims;
using BindingMultipart = Binding.Multipart;

namespace Web.Serialization;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, PropertyNameCaseInsensitive = true)]
// Task types - required for AOT when endpoints return Task (void) or Task<T>
[JsonSerializable(typeof(Task))]
[JsonSerializable(typeof(Task<object>))]
[JsonSerializable(typeof(ValueTask))]
[JsonSerializable(typeof(ValueTask<object>))]
// Primitive/common types needed for minimal APIs and general serialization
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(byte[]))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<object>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(object[]))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
// Admin login DTOs - Request is Web-local (derived from Shared base), Response is Shared
[JsonSerializable(typeof(AdminLogin.Request), TypeInfoPropertyName = "AdminLoginRequest")]
[JsonSerializable(typeof(SharedAdmin.LoginRequestBase), TypeInfoPropertyName = "AdminLoginRequestBase")]
[JsonSerializable(typeof(SharedAdmin.LoginResponse), TypeInfoPropertyName = "AdminLoginResponse")]
[JsonSerializable(typeof(ErrorResponse))]
// Sales DTOs - Requests and Responses are Shared
[JsonSerializable(typeof(SharedSalesOrders.CreateOrderRequest), TypeInfoPropertyName = "SalesOrdersCreateRequest")]
[JsonSerializable(typeof(SharedSalesOrders.CreateOrderResponse), TypeInfoPropertyName = "SalesOrdersCreateResponse")]
[JsonSerializable(typeof(SharedSalesOrders.RetrieveOrderRequest), TypeInfoPropertyName = "SalesOrdersRetrieveRequest")]
[JsonSerializable(typeof(SharedSalesOrders.RetrieveOrderResponse), TypeInfoPropertyName = "SalesOrdersRetrieveResponse")]
// Inventory DTOs - Requests are Web-local (derived from Shared base), Responses are Shared
[JsonSerializable(typeof(Domain.Inventory.Manage.Create.Request), TypeInfoPropertyName = "InventoryManageCreateRequest")]
[JsonSerializable(typeof(Inventory.Manage.Update.Request), TypeInfoPropertyName = "InventoryManageUpdateRequest")]
[JsonSerializable(typeof(Inventory.Manage.Delete.Request), TypeInfoPropertyName = "InventoryManageDeleteRequest")]
[JsonSerializable(typeof(SharedInventory.CreateProductRequestBase), TypeInfoPropertyName = "InventoryCreateProductRequestBase")]
[JsonSerializable(typeof(SharedInventory.UpdateProductRequestBase), TypeInfoPropertyName = "InventoryUpdateProductRequestBase")]
[JsonSerializable(typeof(SharedInventory.DeleteProductRequestBase), TypeInfoPropertyName = "InventoryDeleteProductRequestBase")]
[JsonSerializable(typeof(SharedInventory.CreateProductResponse), TypeInfoPropertyName = "InventoryManageCreateResponse")]
[JsonSerializable(typeof(SharedInventory.GetProductResponse), TypeInfoPropertyName = "InventoryGetProductResponse")]
[JsonSerializable(typeof(SharedInventory.ListRecentInventoryResponse), TypeInfoPropertyName = "InventoryListRecentResponse")]
// Domain customers DTOs - Requests are Web-local (derived from Shared base), Responses are Shared
[JsonSerializable(typeof(Domain.Customers.Create.Request), TypeInfoPropertyName = "CustomersCreateRequest")]
[JsonSerializable(typeof(Domain.Customers.Update.Request), TypeInfoPropertyName = "CustomersUpdateRequest")]
[JsonSerializable(typeof(Domain.Customers.CreateWithPropertiesDI.Request), TypeInfoPropertyName = "CustomersCreateWithPropertiesDIRequest")]
[JsonSerializable(typeof(SharedCustomers.CreateCustomerRequestBase), TypeInfoPropertyName = "CustomersCreateRequestBase")]
[JsonSerializable(typeof(SharedCustomers.UpdateCustomerRequestBase), TypeInfoPropertyName = "CustomersUpdateRequestBase")]
[JsonSerializable(typeof(SharedCustomers.ListRecentCustomersResponse), TypeInfoPropertyName = "CustomersListRecentResponse")]
[JsonSerializable(typeof(SharedCustomers.UpdateCustomerWithHeaderRequest), TypeInfoPropertyName = "CustomersUpdateWithHeaderRequest")]
// Basics/Binding/Pipeline/Versioning sample DTOs - Requests are Shared
[JsonSerializable(typeof(SharedBinding.RouteAndQueryRequest), TypeInfoPropertyName = "BindingRouteAndQueryRequest")]
[JsonSerializable(typeof(SharedPipeline.PipelineRequest), TypeInfoPropertyName = "PipelinePrePostRequest")]
[JsonSerializable(typeof(SharedVersioning.VersionSampleRequest), TypeInfoPropertyName = "VersioningSampleRequest")]
// Upload DTOs - Requests are Web-local (derived from Shared base)
[JsonSerializable(typeof(Uploads.Image.Save.Request), TypeInfoPropertyName = "UploadsImageSaveRequest")]
[JsonSerializable(typeof(Uploads.Image.SaveTyped.Request), TypeInfoPropertyName = "UploadsImageSaveTypedRequest")]
[JsonSerializable(typeof(SharedUploads.ImageSaveRequestBase), TypeInfoPropertyName = "UploadsImageSaveRequestBase")]
[JsonSerializable(typeof(SharedUploads.ImageSaveTypedRequestBase), TypeInfoPropertyName = "UploadsImageSaveTypedRequestBase")]
// Diagnostics endpoint DTOs - Responses are Shared
[JsonSerializable(typeof(SharedSecurity.WhoAmIResponse), TypeInfoPropertyName = "SecurityWhoAmIResponse")]
[JsonSerializable(typeof(SharedSecurity.ClaimKV), TypeInfoPropertyName = "SecurityClaimKV")]
// Security Claims endpoints - Web-local types
[JsonSerializable(typeof(SecurityClaims.VerifyClaimRequest), TypeInfoPropertyName = "SecurityVerifyClaimRequest")]
[JsonSerializable(typeof(SecurityClaims.VerifyClaimResponse), TypeInfoPropertyName = "SecurityVerifyClaimResponse")]
// Binding Multipart - Web-local types
[JsonSerializable(typeof(BindingMultipart.UploadRequest), TypeInfoPropertyName = "BindingMultipartUploadRequest")]
[JsonSerializable(typeof(SharedUploads.MultipartUploadRequestBase), TypeInfoPropertyName = "MultipartUploadRequestBase")]
// FastEndpoints built-in types
[JsonSerializable(typeof(EmptyRequest), TypeInfoPropertyName = "FastEndpointsEmptyRequest")]
[JsonSerializable(typeof(EmptyResponse), TypeInfoPropertyName = "FastEndpointsEmptyResponse")]
public sealed partial class AppJsonContext : JsonSerializerContext { }
