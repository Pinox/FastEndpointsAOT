using System.Text.Json.Serialization;
using MinimalApiAOT;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.TypeInfoResolverChain.Add(AppJsonSerializerContext.Default));

var app = builder.Build();
app.MapGet("/ready", () => Results.Ok()); // Health check for startup measurement
app.MapPost("/benchmark/ok/{id}", (int id, Request req) => Results.Ok(new Response
{
    Id = id,
    Name = req.FirstName + " " + req.LastName,
    Age = req.Age,
    PhoneNumber = req.PhoneNumbers?.FirstOrDefault()
}));

app.Run();

namespace MinimalApiAOT
{
    public class Program { }

    public class Request
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public IEnumerable<string>? PhoneNumbers { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? PhoneNumber { get; set; }
    }

    [JsonSerializable(typeof(Request))]
    [JsonSerializable(typeof(Response))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class AppJsonSerializerContext : JsonSerializerContext;
}
