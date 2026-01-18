using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

namespace Web.Features.Docs
{
    public static class DocsFeature
    {
        public static IServiceCollection AddDocsServices(this IServiceCollection services, IConfiguration config)
        {
            // Use native .NET OpenAPI (AOT-compatible) instead of NSwag
            // Note: Requires Task types to be registered in AppJsonContext for AOT
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info.Title = "Api";
                    document.Info.Version = "v1";
                    return Task.CompletedTask;
                });
            });

            return services;
        }

        public static IApplicationBuilder UseDocsUIIfNonProduction(this IApplicationBuilder app)
        {
            var web = (WebApplication)app;
            
            // Native .NET OpenAPI endpoint - works with AOT when Task types are in JSON context
            web.MapOpenApi();
            
            // Scalar UI pointing to native OpenAPI endpoint
            web.MapScalarApiReference("/api", o =>
            {
                o.WithTitle("Api");
                o.WithOpenApiRoutePattern("/openapi/{documentName}.json");
            });
            
            return app;
        }
    }
}
