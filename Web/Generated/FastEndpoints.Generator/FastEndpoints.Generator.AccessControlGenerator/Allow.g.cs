#nullable enable

using FastEndpoints;

namespace Web.Auth;

public static partial class Allow
{

#region ACL_ITEMS
    /// <summary>Permission for creating a new customer in the system.</summary><remark>Generated from endpoint: <see cref="Domain.Customers.Create.Endpoint"/></remark>
    public const string Customers_Create = "JHH";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Domain.Customers.CreateWithPropertiesDI.Endpoint"/></remark>
    public const string Customers_Create_2 = "B6R";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Domain.Customers.List.Recent.Endpoint"/></remark>
    public const string Customers_Retrieve = "UUH";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Domain.Customers.Update.Endpoint"/></remark>
    public const string Customers_Update = "GR2";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Uploads.Image.SaveTyped.Endpoint"/></remark>
    public const string Image_Update = "O2O";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Domain.Inventory.Manage.Create.Endpoint"/></remark>
    public const string Inventory_Create_Item = "3YI";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Inventory.Manage.Delete.Endpoint"/></remark>
    public const string Inventory_Delete_Item = "PIZ";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Domain.Inventory.GetProduct.Endpoint"/></remark>
    public const string Inventory_Retrieve_Item = "IBU";
    /// <summary></summary><remark>Generated from endpoint: <see cref="Inventory.Manage.Update.Endpoint"/></remark>
    public const string Inventory_Update_Item = "NLM";
    /// <summary>Allows creation of orders by anybody who has this permission code.</summary><remark>Generated from endpoint: <see cref="Domain.Sales.Orders.Create.Endpoint"/></remark>
    public const string Sales_Order_Create = "LYO";
#endregion

#region GROUPS
    public static IEnumerable<string> Admin => _admin;
    private static void AddToAdmin(string permissionCode) => _admin.Add(permissionCode);
    private static readonly List<string>_admin = new()
    {
        Customers_Create,
        Customers_Retrieve,
        Customers_Update,
        Inventory_Create_Item,
        Inventory_Delete_Item,
        Inventory_Retrieve_Item,
        Inventory_Update_Item,
    };

    public static IEnumerable<string> Manager => _manager;
    private static void AddToManager(string permissionCode) => _manager.Add(permissionCode);
    private static readonly List<string>_manager = new()
    {
        Customers_Create,
    };
#endregion

#region DESCRIPTIONS
    [HideFromDocs]
    public static Dictionary<string, string> Descriptions = new()
    {
        //Customers_Create
        { "JHH", "Permission for creating a new customer in the system." },
        //Sales_Order_Create
        { "LYO", "Allows creation of orders by anybody who has this permission code." },
    };
#endregion

}