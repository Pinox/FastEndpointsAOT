using System.Text.Json.Serialization;

namespace FastEndpointsNativeAOT;

[JsonSerializable(typeof(Request))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(Task))]
[JsonSerializable(typeof(Task<Response>))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class BenchSerializerContext : JsonSerializerContext;
