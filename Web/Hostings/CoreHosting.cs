using Web.Serialization;

namespace Web.Hostings;

public static class CoreHosting
{
    // Core services used by the app
    public static IServiceCollection AddCoreAppServices(this IServiceCollection services)
    {
        Console.WriteLine("[DEBUG] AddCoreAppServices called");
        services.AddCors();
        Console.WriteLine("[DEBUG] AddCors completed");
        services.AddOutputCache();
        Console.WriteLine("[DEBUG] AddOutputCache completed");
        services.AddIdempotency();
        Console.WriteLine("[DEBUG] AddIdempotency completed");
        services.AddResponseCaching();
        Console.WriteLine("[DEBUG] AddResponseCaching completed");

        // Configure HttpJsonOptions for AOT compatibility.
        // ASP.NET Core's RequestDelegateFactory uses this when building endpoint routing.
        // FastEndpoints uses its own serializer at runtime, but this is needed at startup.
        services.ConfigureHttpJsonOptions(options =>
        {
            MyResolver.Configure(options.SerializerOptions);
        });
        Console.WriteLine("[DEBUG] ConfigureHttpJsonOptions completed");

        Console.WriteLine("[DEBUG] AddCoreAppServices completed");
        return services;
    }

    /// <summary>
    /// Adds the AOT response buffering middleware that catches VoidTaskResult serialization exceptions.
    /// This must be registered early in the pipeline, before FastEndpoints.
    /// </summary>
    public static IApplicationBuilder UseAotResponseBuffering(this IApplicationBuilder app)
    {
        Console.WriteLine("[DEBUG] UseAotResponseBuffering called");
        var result = app.UseMiddleware<AotResponseBufferingMiddleware>();
        Console.WriteLine("[DEBUG] UseAotResponseBuffering completed");
        return result;
    }

    // Pipeline: Localization, exception handling, response caching, routing, CORS
    public static IApplicationBuilder UseLocalizationConfigured(this IApplicationBuilder app)
    {
        Console.WriteLine("[DEBUG] UseLocalizationConfigured called");
        var result = app.UseRequestLocalization(options =>
        {
            // Keep default en-US as before
            //options.SetDefaultCulture("en-US");
            //options.AddSupportedCultures("en-US");
            //options.AddSupportedUICultures("en-US");
        });
        Console.WriteLine("[DEBUG] UseLocalizationConfigured completed");
        return result;
    }

    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        Console.WriteLine("[DEBUG] UseExceptionHandling called");
        var result = app.UseDefaultExceptionHandler();
        Console.WriteLine("[DEBUG] UseExceptionHandling completed");
        return result;
    }

    public static IApplicationBuilder UseResponseCachingRoutingCors(this IApplicationBuilder app)
    {
        Console.WriteLine("[DEBUG] UseResponseCachingRoutingCors called");
        var result = app
            .UseResponseCaching()
            .UseRouting()
            .UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        Console.WriteLine("[DEBUG] UseResponseCachingRoutingCors completed");
        return result;
    }
}