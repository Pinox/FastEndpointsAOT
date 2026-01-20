using FastEndpoints;
using FEBenchSourceGen;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
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
        c.Serializer.Options.TypeInfoResolverChain.Add(BenchSerializerContext.Default);
    });
app.Run();

namespace FEBenchSourceGen
{
    public class Program { }
}
