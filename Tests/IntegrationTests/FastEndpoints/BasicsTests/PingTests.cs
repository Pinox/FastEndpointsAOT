namespace Int.FastEndpoints.BasicsTests;

public class PingTests(Sut App) : TestBase<Sut>
{
    [Fact]
    public async Task Ping_ReturnsOk()
    {
        // Act
        var response = await App.GuestClient.GetAsync("/api/ping");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("pong");
    }
}
