using FastEndpoints.Swagger;
using NSwag;
using NSwag.Generation.Processors.Security;
using Scalar.AspNetCore;
using Web.Hostings.Processors;

namespace Web.Hostings;

public static class ApiDocsHosting
{
    // Services: Register OpenAPI document generator configuration
    public static IServiceCollection AddApiDocsServices(this IServiceCollection services, IConfiguration config)
    {
        var cookieName = config["Auth:Cookies:Name"] ?? "auth";

        services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.DocumentName = "v1";
                s.Title = "FastEndPointsAPI";
                s.Version = "v1";
                s.SchemaSettings.SchemaType = NJsonSchema.SchemaType.OpenApi3;

                s.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    "JWT",
                    new[] { "JWT" },
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Name = "Authorization",
                        Description = "Bearer {token}"
                    }));
                s.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));

                s.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    ApiKeyAuth.Scheme,
                    new[] { ApiKeyAuth.Scheme },
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Name = ApiKeyAuth.HeaderName
                    }));
                s.OperationProcessors.Add(new OperationSecurityScopeProcessor(ApiKeyAuth.Scheme));

                s.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    "Cookies",
                    new[] { "Cookies" },
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        In = OpenApiSecurityApiKeyLocation.Cookie,
                        Name = cookieName
                    }));
                s.OperationProcessors.Add(new OperationSecurityScopeProcessor("Cookies"));

                // enforce explicit per-endpoint headings + centralized tags list
                s.DocumentProcessors.Add(new SingleTagDocumentProcessor());
                s.OperationProcessors.Add(new SingleTagOperationProcessor());

                // Set summaries to route path and stable OperationIds
                s.OperationProcessors.Add(new FriendlyNameOperationProcessor());
            };

            // Do not transform tag casing; keep exactly what our processors emit
            o.TagCase = TagCase.None;
            o.TagStripSymbols = true;
            o.RemoveEmptyRequestSchema = false;
        });

        return services;
    }

    // Pipeline: UI for OpenAPI + Scalar (non-production)
    public static IApplicationBuilder UseApiDocsUIIfNonProduction(this IApplicationBuilder app)
    {
        var web = (WebApplication)app;
        if (!web.Environment.IsProduction())
        {
            web.UseSwaggerGen();
            web.MapScalarApiReference("/api", o =>
            {
                o.WithTitle("FastEndPointsAPI");
                o.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
            });
        }
        return app;
    }
}
