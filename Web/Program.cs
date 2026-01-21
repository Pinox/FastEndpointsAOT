using sq.Hostings;
using Web.Features.Docs;
using Web.Hostings;

try
{
    var bld = WebApplication.CreateBuilder(args);

    // services (keep order)
    bld.Services
       .AddCoreAppServices()
       .AddFastEndpointsServices()
       .AddAuthNAuthZServices(bld.Configuration)
       .AddInfrastructureServices()
       .AddDocsServices(bld.Configuration) // Native .NET OpenAPI (AOT-compatible)
       .AddRuntimeAotAndEnv();

    // app pipeline (keep order)
    var app = bld.Build();

    app
       // .UseAotResponseBuffering() // Must be early to catch VoidTaskResult exceptions from FastEndpoints
       .UseLocalizationConfigured()
       .UseExceptionHandling()
       .UseResponseCachingRoutingCors()
       .UseJwtRevocationPipeline();

    // Auth middleware is required for protected endpoints
    app.UseAuthNAuthZPipeline();

    app
       .UseAntiforgeryPipeline()
       .UseOutputCachePipeline()
       .UseFastEndpointsPipeline()
       // .MapAdditionalEndpoints() // Disabled: Minimal API endpoints not compatible with AOT without additional JSON context config
       .UseDocsUIIfNonProduction();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"[CRITICAL ERROR] Application startup failed: {ex.Message}");
    Console.WriteLine($"[CRITICAL ERROR] Stack trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"[CRITICAL ERROR] Inner exception: {ex.InnerException.Message}");
        Console.WriteLine($"[CRITICAL ERROR] Inner stack trace: {ex.InnerException.StackTrace}");
    }
    throw; // Re-throw to ensure the app exits with error code
}