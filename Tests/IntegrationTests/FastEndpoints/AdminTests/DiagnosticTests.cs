using System.Net;
using System.Net.Http.Json;

namespace Int.FastEndpoints.AdminTests;

public class DiagnosticTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task Login_DirectCall_ShowResponseBody()
    {
        // Call the endpoint directly using the raw HttpClient
        var client = App.GuestClient;
        
        var request = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { email = "admin@email.com", Password = "pass" }),
            System.Text.Encoding.UTF8,
            "application/json");
        
        var response = await client.PostAsync("/api/admin/login/ver1", request);
        
        var body = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"STATUS: {response.StatusCode}");
        Console.WriteLine($"BODY: {body}");
        Console.WriteLine($"HEADERS: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(",", h.Value)}"))}");
        
        // This will fail but show us the info
        response.StatusCode.ShouldBe(HttpStatusCode.OK, $"Response body: {body}");
    }
}
