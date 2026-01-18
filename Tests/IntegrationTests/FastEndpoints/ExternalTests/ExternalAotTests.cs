using System.Net;
using Shared;
using Shared.Contracts.Admin;
using Shared.Contracts.Customers;
using Shared.Contracts.Inventory;
using Xunit;

namespace Int.FastEndpoints.ExternalTests;

/// <summary>
/// Tests that run against an externally running AOT-compiled application.
/// 
/// To run these tests:
/// 1. Publish: dotnet publish Web/Web.csproj -c Release -o ./publish
/// 2. Start the app: ./publish/Web.exe (or Web on Linux/Mac)
/// 3. Set URL: $env:EXTERNAL_API_URL = "http://localhost:5000"
/// 4. Run: dotnet test --filter "Category=External"
/// </summary>
[Trait("Category", "External")]
public class ExternalAotTests : IClassFixture<ExternalSut>
{
    private readonly ExternalSut _app;

    public ExternalAotTests(ExternalSut app)
    {
        _app = app;
    }

    [Fact]
    public async Task AdminLogin_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange & Act - Using IApiRequest pattern
        var (response, result) = await _app.GuestClient.SendAsync(new LoginRequest
        {
            email = "admin@email.com",
            Password = "pass"
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.JWTToken));
    }

    [Fact]
    public async Task ListRecentCustomers_AsAdmin_ReturnsOk()
    {
        // Act - Using IApiRequest pattern
        var (response, _) = await _app.AdminClient.SendAsync(new ListRecentCustomersRequest());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListRecentInventory_AsAdmin_ReturnsOk()
    {
        // Act - Using IApiRequest pattern with route parameter
        var (response, _) = await _app.AdminClient.SendAsync(new ListRecentInventoryRequest { CategoryID = "CAT-001" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_AsGuest_ReturnsUnauthorized()
    {
        // Act - Using IApiRequest pattern
        var (response, _) = await _app.GuestClient.SendAsync(new ListRecentCustomersRequest());

        // Assert - Should be unauthorized or forbidden without JWT
        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized || 
            response.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 401/403 but got {response.StatusCode}");
    }
}
