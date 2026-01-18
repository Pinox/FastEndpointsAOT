namespace OpenApi;

public class OpenApiDocTests(Fixture App) : TestBase<Fixture>
{
    // NOTE: Snapshot testing for native .NET OpenAPI endpoint
    //       To update the golden master (verified json files), set '_updateSnapshots = true' and run the tests.
    //       Don't forget to set it back to 'false' afterward.

    static readonly bool _updateSnapshots = false;

    [Fact]
    public async Task OpenApi_V1_Endpoint_Returns_Valid_Json()
    {
        // Act
        var response = await App.Client.GetAsync("/openapi/v1.json", Cancellation);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var json = await response.Content.ReadAsStringAsync(Cancellation);
        json.ShouldNotBeNullOrWhiteSpace();

        // Verify it's valid JSON by parsing
        var doc = JToken.Parse(json);
        doc.ShouldNotBeNull();

        // Verify basic OpenAPI structure
        doc["openapi"]?.ToString().ShouldStartWith("3.");
        doc["info"].ShouldNotBeNull();
        doc["paths"].ShouldNotBeNull();
    }

    [Fact]
    public async Task OpenApi_V1_Contains_Expected_Endpoints()
    {
        // Act
        var response = await App.Client.GetAsync("/openapi/v1.json", Cancellation);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(Cancellation);
        var doc = JToken.Parse(json);

        // Assert - verify some expected endpoints exist
        var paths = doc["paths"] as JObject;
        paths.ShouldNotBeNull();

        // Should have multiple endpoints registered
        paths.Count.ShouldBeGreaterThan(10);
    }

    [Fact]
    public async Task OpenApi_V1_Document_Snapshot()
    {
        // Act
        var response = await App.Client.GetAsync("/openapi/v1.json", Cancellation);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(Cancellation);
        var currentDoc = JToken.Parse(json);

        await UpdateSnapshotIfEnabled("openapi-v1.json", json);

        // Assert against snapshot
        var snapshotPath = "openapi-v1.json";
        if (!File.Exists(snapshotPath))
        {
            // If snapshot doesn't exist yet, create it and skip comparison
            await File.WriteAllTextAsync(snapshotPath, json, Cancellation);
            return;
        }

        var snapshot = await File.ReadAllTextAsync(snapshotPath, Cancellation);
        var snapshotDoc = JToken.Parse(snapshot);

        currentDoc.ShouldBeEquivalentTo(snapshotDoc);
    }

    [Fact]
    public async Task OpenApi_V1_Contains_Components_Schemas()
    {
        // Act
        var response = await App.Client.GetAsync("/openapi/v1.json", Cancellation);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(Cancellation);
        var doc = JToken.Parse(json);

        // Assert - verify components/schemas section exists
        var components = doc["components"];
        components.ShouldNotBeNull();

        var schemas = components["schemas"] as JObject;
        schemas.ShouldNotBeNull();
        schemas.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Scalar_UI_Endpoint_Returns_Html()
    {
        // Act
        var response = await App.Client.GetAsync("/api", Cancellation);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");

        var html = await response.Content.ReadAsStringAsync(Cancellation);
        html.ShouldNotBeNullOrWhiteSpace();
        html.ShouldContain("scalar", Case.Insensitive);
    }

    static async Task UpdateSnapshotIfEnabled(string jsonFileName, string jsonContent)
    {
        if (_updateSnapshots is false)
            return;

        var destination = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", jsonFileName));

        await File.WriteAllTextAsync(destination, jsonContent);

        throw new OperationCanceledException($"Snapshots updated at {destination}! Set _updateSnapshots = false and re-run the tests!");
    }
}
