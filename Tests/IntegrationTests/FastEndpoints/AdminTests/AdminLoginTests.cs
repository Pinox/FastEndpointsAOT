using Admin.Login;
using Shared.Contracts.Admin;

namespace Int.FastEndpoints.AdminTests;

public class AdminLoginTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task AdminLogin_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var request = new Request
        {
            email = "admin@email.com",
            Password = "pass"
        };

        // Act
        var (rsp, res) = await App.GuestClient.POSTAsync<Endpoint_V1, Request, LoginResponse>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.JWTToken.ShouldNotBeNullOrEmpty();
        res.ExpiryDate.ShouldBeGreaterThan(DateTime.UtcNow);
        res.Permissions.ShouldNotBeNull();
    }

    [Fact]
    public async Task AdminLogin_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = new Request
        {
            email = "wrong@email.com",
            Password = "wrongpassword"
        };

        // Act
        var (rsp, _) = await App.GuestClient.POSTAsync<Endpoint_V1, Request, LoginResponse>(request);

        // Assert - Endpoint returns 400 Bad Request with error for invalid credentials
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminLogin_V2_ReturnsTwo()
    {
        // Act - Use direct HTTP for GET (avoids EmptyRequest AOT serialization issue)
        // Version 2 route: /api/admin/login/ver2
        var rsp = await App.GuestClient.GetAsync("/api/admin/login/ver2");

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await rsp.Content.ReadAsStringAsync();
        content.ShouldBe("2");
    }
}
