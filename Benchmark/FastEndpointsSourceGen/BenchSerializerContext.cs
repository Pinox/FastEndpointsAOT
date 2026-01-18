using System.Text.Json.Serialization;

namespace FEBenchSourceGen;

[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class BenchSerializerContext : JsonSerializerContext;
