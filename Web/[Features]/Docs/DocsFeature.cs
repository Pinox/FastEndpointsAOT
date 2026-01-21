using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

namespace Web.Features.Docs
{
    public static class DocsFeature
    {
        public static IServiceCollection AddDocsServices(this IServiceCollection services, IConfiguration config)
        {
            // Use native .NET OpenAPI (AOT-compatible) instead of NSwag
            // Note: Requires all request/response types to be registered in AppJsonContext for AOT
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info.Title = "Api";
                    document.Info.Version = "v1";
                    return Task.CompletedTask;
                });
                
                // Exclude test case endpoints from OpenAPI schema to avoid AOT serialization issues
                // TestCase endpoints use many types that would all need to be in AppJsonContext
                // The ShouldInclude delegate controls which endpoints are processed for schema generation
                options.ShouldInclude = (description) =>
                {
                    var relativePath = description.RelativePath?.TrimStart('/') ?? "";
                    
                    // Strip common prefixes (api/, mobile/, etc.) to check the actual route
                    if (relativePath.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
                        relativePath = relativePath.Substring(4);
                    else if (relativePath.StartsWith("mobile/api/", StringComparison.OrdinalIgnoreCase))
                        relativePath = relativePath.Substring(11);
                    else if (relativePath.StartsWith("mobile/", StringComparison.OrdinalIgnoreCase))
                        relativePath = relativePath.Substring(7);
                    
                    // Exclude test case paths - they have many unregistered types
                    // Patterns: test-cases/, testcases/, test/, tests/, multi-test (TypedResultTest)
                    if (relativePath.StartsWith("test-cases/", StringComparison.OrdinalIgnoreCase) ||
                        relativePath.StartsWith("testcases/", StringComparison.OrdinalIgnoreCase) ||
                        relativePath.StartsWith("test/", StringComparison.OrdinalIgnoreCase) ||
                        relativePath.StartsWith("tests/", StringComparison.OrdinalIgnoreCase) ||
                        relativePath.StartsWith("multi-test", StringComparison.OrdinalIgnoreCase) ||
                        relativePath == "test")
                    {
                        return false;
                    }
                    
                    // Exclude endpoints with Hide tag or HideFromDocs attribute
                    var metadata = description.ActionDescriptor.EndpointMetadata;
                    foreach (var item in metadata)
                    {
                        // Check for HideFromDocs attribute
                        if (item?.GetType().Name == "HideFromDocsAttribute")
                            return false;
                        
                        // Check for Tags with "Hide" value (FastEndpoints WithTags("Hide"))
                        if (item is Microsoft.AspNetCore.Http.Metadata.ITagsMetadata tagsMetadata)
                        {
                            if (tagsMetadata.Tags.Contains("Hide"))
                                return false;
                        }
                    }
                    
                    return true;
                };
            });

            return services;
        }

        public static IApplicationBuilder UseDocsUIIfNonProduction(this IApplicationBuilder app)
        {
            var web = (WebApplication)app;
            
            // Map OpenAPI and Scalar UI for API documentation
            web.MapOpenApi();
            
            web.MapScalarApiReference("/api", o =>
            {
                o.Title = "Api";
                o.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                o.AddServer(new ScalarServer("http://localhost:5000", "Development"));
            });
            
            return app;
        }
    }
}
