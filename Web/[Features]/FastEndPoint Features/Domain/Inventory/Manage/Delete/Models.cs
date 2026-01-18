using System.ComponentModel;
using Shared.Contracts.Inventory;

namespace Inventory.Manage.Delete;

/// <summary>
/// Derived request with Web-specific [DefaultValue] attribute.
/// </summary>
public class Request : DeleteProductRequestBase
{
    [DefaultValue("test default val")]
    public override string ItemID { get; set; } = "";
}
