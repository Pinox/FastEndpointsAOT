namespace Shared;

/// <summary>
/// API route constants shared between server and clients.
/// These routes are used by IApiRequest implementations for type-safe HTTP calls.
/// </summary>
public static class ApiRoutes
{
    // Admin
    public const string Admin_Login = "/api/admin/login/ver1";

    // Customers
    public const string Customer_ListRecent = "/api/customer/list/recent";
    public const string Customer_Create = "/api/customer/new";
    public const string Customer_Update = "/api/customer/update";
    public const string Customer_UpdateWithHeader = "/api/customer/update-with-header";

    // Inventory
    public const string Inventory_Create = "/api/inventory/manage/create";
    public const string Inventory_Update = "/api/inventory/manage/update";
    public const string Inventory_Delete = "/api/inventory/manage/delete/{itemID}";
    public const string Inventory_GetProduct = "/api/inventory/get-product/{ProductID}";
    public const string Inventory_ListRecent = "/api/inventory/list/recent/{CategoryID}";

    // Sales Orders
    public const string Sales_Orders_Create = "/sales/orders/create";
    public const string Sales_Orders_Retrieve = "/sales/orders/retrieve/{OrderID}";

    // Security
    public const string Security_WhoAmI = "/security/whoami";
    public const string Security_Claims_Verify = "/security/claims/verify";

    // Binding samples
    public const string Binding_RouteAndQuery = "/samples/binding/items/{Id}";
}
