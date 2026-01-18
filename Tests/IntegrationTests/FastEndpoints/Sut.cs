using Admin.Login;
using Shared.Contracts.Admin;
using Web.Services;

namespace Int.FastEndpoints;

/// <summary>
/// System Under Test - spins up the Web application for integration testing.
/// </summary>
public class Sut : AppFixture<Web.Program>
{
    /// <summary>Unauthenticated client</summary>
    public HttpClient GuestClient { get; private set; } = default!;
    
    /// <summary>Admin user with JWT token</summary>
    public HttpClient AdminClient { get; private set; } = default!;

    protected override async ValueTask SetupAsync()
    {
        // Guest client - no auth
        GuestClient = CreateClient();
        // Add unique throttle identifier for rate limiting
        GuestClient.DefaultRequestHeaders.Add("X-Custom-Throttle-Header", $"guest-{Guid.NewGuid()}");

        // Admin client - authenticate and get JWT
        AdminClient = CreateClient();
        // Add unique throttle identifier for rate limiting
        AdminClient.DefaultRequestHeaders.Add("X-Custom-Throttle-Header", $"admin-{Guid.NewGuid()}");
        
        // Login to get JWT token
        var (rsp, result) = await AdminClient.POSTAsync<
                              Endpoint_V1,
                              Request,
                              LoginResponse>(
                              new()
                              {
                                  email = "admin@email.com",
                                  Password = "pass"
                              });
        
        // Debug: log if login failed
        if (!rsp.IsSuccessStatusCode)
        {
            var body = await rsp.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"[Sut.SetupAsync] Login failed: {rsp.StatusCode} - {body}");
        }
        
        if (result?.JWTToken is not null)
        {
            AdminClient.DefaultRequestHeaders.Authorization = new("Bearer", result.JWTToken);
        }
        else
        {
            throw new InvalidOperationException("[Sut.SetupAsync] Login succeeded but no JWT token was returned!");
        }
        // SecurityProcessor requires tenant-id = "qwerty" for certain endpoints
        AdminClient.DefaultRequestHeaders.Add("tenant-id", "qwerty");
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // Mock external services
        s.AddScoped<IEmailService, MockEmailService>();
    }

    protected override ValueTask TearDownAsync()
    {
        GuestClient.Dispose();
        AdminClient.Dispose();
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Mock email service for testing
/// </summary>
public class MockEmailService : IEmailService
{
    public string SendEmail() => "mock email sent";
}
