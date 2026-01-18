namespace Web.Auth;

public static partial class Allow
{
    public const string Additional_Permission = "_AP1";
    public const string Another_Permission = "_AP2";

    static partial void Groups()
    {
        // Add all required permissions to the Admin group for integration testing
        AddToAdmin(Additional_Permission);
        AddToAdmin(Sales_Order_Create);
        AddToAdmin(Inventory_Create_Item);
        AddToAdmin(Inventory_Update_Item);
        AddToManager(Another_Permission);
    }

    static partial void Describe()
    {
        Descriptions[Additional_Permission] = "Description for first custom permission";
        Descriptions[Another_Permission] = "Another custom permission";
        Descriptions[Inventory_Create_Item] = "Descriptions for generated permission";
    }
}