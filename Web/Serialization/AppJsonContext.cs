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

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    PropertyNameCaseInsensitive = true)]
// Task types - required for AOT when endpoints return Task (void) or Task<T>
[JsonSerializable(typeof(Task))]
[JsonSerializable(typeof(Task<object>))]
[JsonSerializable(typeof(ValueTask))]
[JsonSerializable(typeof(ValueTask<object>))]
// Primitive types and their nullable versions
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(bool?))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(byte?))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(sbyte?))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(short?))]
[JsonSerializable(typeof(ushort))]
[JsonSerializable(typeof(ushort?))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(uint?))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(long?))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(ulong?))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(float?))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(double?))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(char))]
[JsonSerializable(typeof(char?))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(byte[]))]
[JsonSerializable(typeof(object))]
// Date/Time types
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(DateTimeOffset?))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(TimeSpan?))]
[JsonSerializable(typeof(DateOnly))]
[JsonSerializable(typeof(DateOnly?))]
[JsonSerializable(typeof(TimeOnly))]
[JsonSerializable(typeof(TimeOnly?))]
// Other common types
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(Version))]
// Collection types
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<object>))]
[JsonSerializable(typeof(IEnumerable<Guid>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(object[]))]
[JsonSerializable(typeof(Guid[]))]
[JsonSerializable(typeof(List<bool>))]
[JsonSerializable(typeof(List<int>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<Guid>))]
[JsonSerializable(typeof(List<DateTime>))]
// Dictionary types
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSerializable(typeof(Dictionary<string, bool>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
// FastEndpoints library types needed for OpenAPI schema generation
[JsonSerializable(typeof(FastEndpoints.Security.TokenRequest))]
[JsonSerializable(typeof(FastEndpoints.Security.TokenResponse))]
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
// TestCases DTOs - for AOT compatibility
[JsonSerializable(typeof(TestCases.CustomRequestBinder.Product), TypeInfoPropertyName = "CustomRequestBinderProduct")]
[JsonSerializable(typeof(TestCases.CustomRequestBinder.Request), TypeInfoPropertyName = "CustomRequestBinderRequest")]
[JsonSerializable(typeof(TestCases.CustomRequestBinder.Response), TypeInfoPropertyName = "CustomRequestBinderResponse")]
[JsonSerializable(typeof(TestCases.AntiforgeryTest.TokenResponse), TypeInfoPropertyName = "AntiforgeryTestTokenResponse")]
[JsonSerializable(typeof(TestCases.AntiforgeryTest.VerificationRequest), TypeInfoPropertyName = "AntiforgeryTestVerificationRequest")]
// FastEndpoints built-in types
[JsonSerializable(typeof(EmptyRequest), TypeInfoPropertyName = "FastEndpointsEmptyRequest")]
[JsonSerializable(typeof(EmptyResponse), TypeInfoPropertyName = "FastEndpointsEmptyResponse")]
[JsonSerializable(typeof(FastEndpoints.InternalErrorResponse), TypeInfoPropertyName = "FastEndpointsInternalErrorResponse")]
public sealed partial class AppJsonContext : JsonSerializerContext { }
