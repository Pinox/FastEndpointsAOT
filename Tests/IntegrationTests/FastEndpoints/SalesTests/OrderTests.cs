using Shared.Contracts.Sales.Orders;

namespace Int.FastEndpoints.SalesTests;

public class OrderTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsOrderId()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerID = 123,
            ProductID = 456,
            Quantity = 2
        };

        // Act
        var (rsp, res) = await App.AdminClient.POSTAsync<
            Domain.Sales.Orders.Create.Endpoint,
            CreateOrderRequest,
            CreateOrderResponse>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.OrderID.ShouldNotBe(0);
    }

    [Fact]
    public async Task RetrieveOrder_WithValidId_ReturnsOrder()
    {
        // Arrange
        var request = new RetrieveOrderRequest
        {
            OrderID = "ORD-001"
        };

        // Act
        var (rsp, res) = await App.AdminClient.GETAsync<
            Domain.Sales.Orders.Retrieve.Endpoint,
            RetrieveOrderRequest,
            RetrieveOrderResponse>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateOrder_AsGuest_RequiresAuthentication()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerID = 123,
            ProductID = 456,
            Quantity = 1
        };

        // Act
        var (rsp, _) = await App.GuestClient.POSTAsync<
            Domain.Sales.Orders.Create.Endpoint,
            CreateOrderRequest,
            CreateOrderResponse>(request);

        // Assert - should require authentication
        rsp.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }
}
