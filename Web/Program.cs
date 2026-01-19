using sq.Hostings;
using Web.Features.Docs;
using Web.Hostings;
using Web.Infrastructure;

try
{
    Console.WriteLine("[DEBUG] Starting application...");
    
    // Ensure AOT trimmer preserves endpoint method metadata
    AotEndpointPreserver.EnsureEndpointsPreserved();

    var bld = WebApplication.CreateBuilder(args);
    Console.WriteLine("[DEBUG] WebApplication.CreateBuilder completed");

    // services (keep order)
    Console.WriteLine("[DEBUG] Adding services...");
    bld.Services
       .AddCoreAppServices()
       .AddFastEndpointsServices()
       .AddAuthNAuthZServices(bld.Configuration)
       .AddInfrastructureServices()
       .AddDocsServices(bld.Configuration) // Native .NET OpenAPI (AOT-compatible)
       .AddRuntimeAotAndEnv();
    Console.WriteLine("[DEBUG] Services added successfully");

    // app pipeline (keep order)
    Console.WriteLine("[DEBUG] Building app...");
    var app = bld.Build();
    Console.WriteLine("[DEBUG] App built successfully");

    Console.WriteLine("[DEBUG] Configuring pipeline...");
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
    Console.WriteLine("[DEBUG] Pipeline configured successfully");

    Console.WriteLine("[DEBUG] Starting app.Run()...");
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