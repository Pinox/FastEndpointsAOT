using Shared;
using Shared.Contracts.Inventory;

namespace Int.FastEndpoints.InventoryTests;

public class InventoryTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new Domain.Inventory.Manage.Create.Request
        {
            Name = "Test Product",
            Description = "A test product description",
            Price = 99.99m,
            ModifiedBy = "test-user"
        };

        // Act
        var (rsp, res) = await App.AdminClient.POSTAsync<
            Domain.Inventory.Manage.Create.Endpoint,
            Domain.Inventory.Manage.Create.Request,
            CreateProductResponse>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.ProductId.ShouldBeGreaterThan(0);
        res.ProductName.ShouldBe("Test Product");
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsValidationError()
    {
        // Arrange - empty name should fail validation
        var request = new Domain.Inventory.Manage.Create.Request
        {
            Name = "", // Invalid
            Description = "Test",
            Price = 10m,
            ModifiedBy = "test"
        };

        // Act
        var (rsp, _) = await App.AdminClient.POSTAsync<
            Domain.Inventory.Manage.Create.Endpoint,
            Domain.Inventory.Manage.Create.Request,
            CreateProductResponse>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = new Inventory.Manage.Update.Request
        {
            Id = 1,
            Name = "Updated Product",
            Description = "Updated description",
            Price = 149.99m,
            ModifiedBy = "test-user"
        };

        // Act
        var (rsp, _) = await App.AdminClient.PUTAsync<
            Inventory.Manage.Update.Endpoint,
            Inventory.Manage.Update.Request,
            object>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsOk()
    {
        // Arrange
        var request = new Inventory.Manage.Delete.Request
        {
            ItemID = "PROD-001"
        };

        // Act
        var (rsp, _) = await App.AdminClient.DELETEAsync<
            Inventory.Manage.Delete.Endpoint,
            Inventory.Manage.Delete.Request,
            object>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListRecentInventory_AsAdmin_ReturnsProducts()
    {
        // Act - Using IApiRequest pattern with SendAsync (route parameter hydrated automatically)
        var (rsp, res) = await App.AdminClient.SendAsync(new ListRecentInventoryRequest { CategoryID = "CAT-001" });

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.Category.ShouldNotBeNull();
    }
}
