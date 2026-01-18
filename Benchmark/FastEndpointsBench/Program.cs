using FastEndpoints;
using FEBench;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddFastEndpoints(
    o =>
    {
        o.DisableAutoDiscovery = true;
        o.Assemblies = [typeof(Program).Assembly];
    });
builder.Services.AddScoped<ScopedValidator>();

var app = builder.Build();
app.UseFastEndpoints(c => c.Binding.ReflectionCache.AddFromFEBench());
app.Run();

namespace FEBench
{
    public class Program { }
}