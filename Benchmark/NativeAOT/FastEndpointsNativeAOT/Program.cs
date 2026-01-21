using FastEndpoints;
using FastEndpointsNativeAOT;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();

// Ensure AOT trimmer preserves endpoint method metadata
AotEndpointPreserver.EnsureEndpointsPreserved();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.TypeInfoResolverChain.Add(BenchSerializerContext.Default));
builder.Services.AddFastEndpoints(
    o =>
    {
        o.DisableAutoDiscovery = true;
        o.Assemblies = [typeof(Program).Assembly];
    });

var app = builder.Build();
app.UseFastEndpoints(
    c =>
    {
        c.Binding.ReflectionCache.AddFromFastEndpointsNativeAOT();
        c.Serializer.Options.TypeInfoResolverChain.Add(BenchSerializerContext.Default);
        
        // Enable source-generated command executors for AOT compatibility
        c.Endpoints.UseGeneratedCommandExecutors = true;
    });
app.Run();

namespace FastEndpointsNativeAOT
{
    public class Program { }
}
