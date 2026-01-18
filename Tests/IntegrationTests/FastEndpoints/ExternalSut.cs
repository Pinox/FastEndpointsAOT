using Shared;
using Shared.Contracts.Admin;
using Xunit;

namespace Int.FastEndpoints;

/// <summary>
/// External System Under Test - connects to an externally running AOT-compiled app.
/// 
/// Usage:
/// 1. Publish the app with AOT: dotnet publish Web/Web.csproj -c Release
/// 2. Run the published binary: ./publish/Web.exe
/// 3. Set environment variable: $env:EXTERNAL_API_URL = "http://localhost:5000"
/// 4. Run tests: dotnet test --filter "Category=External"
/// </summary>
public class ExternalSut : IAsyncLifetime
{
    private readonly string _baseUrl;
    
    /// <summary>Unauthenticated client</summary>
    public HttpClient GuestClient { get; private set; } = default!;
    
    /// <summary>Admin user with JWT token</summary>
    public HttpClient AdminClient { get; private set; } = default!;

    public ExternalSut()
    {
        _baseUrl = Environment.GetEnvironmentVariable("EXTERNAL_API_URL") 
            ?? "http://localhost:5000";
    }

    public async ValueTask InitializeAsync()
    {
        // Guest client - no auth
        GuestClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        GuestClient.DefaultRequestHeaders.Add("X-Custom-Throttle-Header", $"guest-{Guid.NewGuid()}");
        GuestClient.DefaultRequestHeaders.Add("tenant-id", "qwerty");

        // Admin client - authenticate and get JWT
        AdminClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        AdminClient.DefaultRequestHeaders.Add("X-Custom-Throttle-Header", $"admin-{Guid.NewGuid()}");
        AdminClient.DefaultRequestHeaders.Add("tenant-id", "qwerty");
        
        // Login to get JWT token using IApiRequest pattern
        var (response, result) = await AdminClient.SendAsync(new LoginRequest
        {
            email = "admin@email.com",
            Password = "pass"
        });
        
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"[ExternalSut] Login failed: {response.StatusCode} - {body}");
        }
        if (result?.JWTToken is not null)
        {
            AdminClient.DefaultRequestHeaders.Authorization = new("Bearer", result.JWTToken);
        }
        else
        {
            throw new InvalidOperationException("[ExternalSut] Login succeeded but no JWT token was returned!");
        }
    }

    public ValueTask DisposeAsync()
    {
        GuestClient?.Dispose();
        AdminClient?.Dispose();
        return ValueTask.CompletedTask;
    }
}
