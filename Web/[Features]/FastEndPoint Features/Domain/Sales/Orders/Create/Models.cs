namespace Domain.Sales.Orders.Create;

public class Request
{
    public int CustomerID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }

    /// <summary>
    /// this is a guid property description
    /// </summary>
    public Guid GuidTest { get; set; }
}

// Response class moved to Shared.Contracts.Sales.Orders.CreateOrderResponse