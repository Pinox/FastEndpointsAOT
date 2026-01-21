using Web.Serialization;

namespace Web.Hostings;

public static class CoreHosting
{
    // Core services used by the app
    public static IServiceCollection AddCoreAppServices(this IServiceCollection services)
    {
        services.AddCors();
        services.AddOutputCache();
        services.AddIdempotency();
        services.AddResponseCaching();

        // Configure HttpJsonOptions for AOT compatibility.
        // ASP.NET Core's RequestDelegateFactory uses this when building endpoint routing.
        // FastEndpoints uses its own serializer at runtime, but this is needed at startup.
        services.ConfigureHttpJsonOptions(options =>
        {
            MyResolver.Configure(options.SerializerOptions);
        });

        return services;
    }

    /// <summary>
    /// Adds the AOT response buffering middleware that catches VoidTaskResult serialization exceptions.
    /// This must be registered early in the pipeline, before FastEndpoints.
    /// </summary>
    public static IApplicationBuilder UseAotResponseBuffering(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AotResponseBufferingMiddleware>();
    }

    // Pipeline: Localization, exception handling, response caching, routing, CORS
    public static IApplicationBuilder UseLocalizationConfigured(this IApplicationBuilder app)
    {
        return app.UseRequestLocalization(options =>
        {
            // Keep default en-US as before
            //options.SetDefaultCulture("en-US");
            //options.AddSupportedCultures("en-US");
            //options.AddSupportedUICultures("en-US");
        });
    }

    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseDefaultExceptionHandler();
    }

    public static IApplicationBuilder UseResponseCachingRoutingCors(this IApplicationBuilder app)
    {
        return app
            .UseResponseCaching()
            .UseRouting()
            .UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }
}