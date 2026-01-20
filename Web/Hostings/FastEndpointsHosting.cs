using Web;
using Web.PipelineBehaviors.PreProcessors;
using Web.Infrastructure;

namespace sq.Hostings;

public static class FastEndpointsHosting
{
    // Services: Add FastEndpoints with source generator discovered types for AOT compatibility
    public static IServiceCollection AddFastEndpointsServices(this IServiceCollection services)
    {
        // DEBUG: Analyze endpoint types before FastEndpoints processes them
        Console.WriteLine("[DEBUG] About to analyze discovered types...");
        EndpointDebugger.DebugEndpointTypes(DiscoveredTypes.All);
        Console.WriteLine("[DEBUG] Analysis complete, now calling AddFastEndpoints...");
        
        return services.AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
    }

    // Pipeline: Configure FastEndpoints
    public static IApplicationBuilder UseFastEndpointsPipeline(this IApplicationBuilder app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Validation.EnableDataAnnotationsSupport = true;

            c.Binding.UsePropertyNamingPolicy = true;
            c.Binding.ValueParserFor<Guid>(x => new(Guid.TryParse(x, out var res), res));

            MyResolver.Configure(c.Serializer.Options);

            c.Endpoints.RoutePrefix = "api";
            c.Endpoints.ShortNames = false;
            c.Endpoints.PrefixNameWithFirstTag = true;
            c.Endpoints.Filter = ep => ep.EndpointTags?.Contains("exclude") is not true;

            var isDev = (app as WebApplication)!.Environment.IsDevelopment();
            c.Endpoints.Configurator = ep =>
            {
                ep.PreProcessors(Order.Before, new AdminHeaderChecker());
                if (ep.EndpointTags?.Contains("Orders") is true)
                    ep.Description(b => b.Produces<ErrorResponse>(400, "application/problem+json"));

                if (!isDev)
                {
                    if (ep.Routes?.Any(r => r.Contains(AppRoutes.fastendpoints_prefix, StringComparison.OrdinalIgnoreCase)) == true)
                    {
                        ep.Description(b => b.WithTags("exclude"));
                    }
                }
                // do not set default tags here; let endpoint-defined tags or mapping decide
            };

            c.Versioning.Prefix = "ver";

            c.Throttle.HeaderName = "X-Custom-Throttle-Header";
            c.Throttle.Message = "Custom Error Response";
        });

        // Register pre-generated command handler executors for AOT compatibility
        var sp = (app as WebApplication)!.Services;
        sp.RegisterCommandExecutors(GeneratedReflection.RegisterCommandExecutors);
        
        // Enable AOT mode to require pre-generated executors (will throw if missing)
        CommandExtensions.EnableAotMode();

        return app;
    }
}