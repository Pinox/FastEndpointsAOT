using FastEndpoints;
using FastEndpointsNativeAOT;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.TypeInfoResolverChain.Add(BenchSerializerContext.Default));
builder.Services.AddFastEndpoints(
    o =>
    {
        // Use source-generated discovered types for AOT compatibility
        o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
    });

var app = builder.Build();
app.UseFastEndpoints(
    c =>
    {
        c.Binding.ReflectionCache.AddFromFastEndpointsNativeAOT();
        c.Serializer.Options.TypeInfoResolverChain.Add(BenchSerializerContext.Default);
    });
app.Run();

namespace FastEndpointsNativeAOT
{
    public class Program { }
}
