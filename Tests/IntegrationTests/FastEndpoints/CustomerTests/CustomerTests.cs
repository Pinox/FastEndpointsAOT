using Shared;
using Shared.Contracts.Customers;

namespace Int.FastEndpoints.CustomerTests;

public class CustomerTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task ListRecentCustomers_AsAdmin_ReturnsCustomers()
    {
        // Act - Using IApiRequest pattern with SendAsync
        var (rsp, res) = await App.AdminClient.SendAsync(new ListRecentCustomersRequest());
        
        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.Customers.ShouldNotBeNull();
        res.Customers.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task CreateCustomer_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = new Domain.Customers.Create.Request
        {
            CustomerName = "Test Customer",
            PhoneNumbers = ["555-1234", "555-5678"]
        };

        // Act
        var (rsp, _) = await App.AdminClient.POSTAsync<
            Domain.Customers.Create.Endpoint,
            Domain.Customers.Create.Request,
            object>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidData_ReturnsValidationError()
    {
        // Arrange - empty customer name should fail validation
        var request = new Domain.Customers.Create.Request
        {
            CustomerName = "", // Invalid - empty
            PhoneNumbers = []
        };

        // Act
        var (rsp, _) = await App.AdminClient.POSTAsync<
            Domain.Customers.Create.Endpoint,
            Domain.Customers.Create.Request,
            object>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCustomer_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = new Domain.Customers.Update.Request
        {
            CustomerID = "123",
            Name = "Updated Customer Name",
            Age = 30,
            Address = "123 Test Street"
        };

        // Act
        var (rsp, _) = await App.AdminClient.PUTAsync<
            Domain.Customers.Update.Endpoint,
            Domain.Customers.Update.Request,
            object>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
