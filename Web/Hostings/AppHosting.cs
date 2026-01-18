using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Web;
using Web.PipelineBehaviors.PreProcessors;

namespace Web.Hostings;

public static class AppHosting
{
    // Compose the HTTP pipeline exactly in the previous order
    public static WebApplication UseAppPipeline(this WebApplication app)
    {
        var supportedCultures = new[] { new CultureInfo("en-US") };

        app.UseRequestLocalization(
               new RequestLocalizationOptions
               {
                   DefaultRequestCulture = new("en-US"),
                   SupportedCultures = supportedCultures,
                   SupportedUICultures = supportedCultures
               })
           .UseDefaultExceptionHandler()
           .UseResponseCaching()
           .UseRouting() // must go before auth/cors/fastendpoints middleware
           .UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
           .UseJwtRevocation<JwtBlacklistChecker>()
           .UseAuthentication()
           .UseAuthorization()
           .UseAntiforgeryFE(additionalContentTypes: ["application/json"])
           .UseOutputCache()
           .UseFastEndpoints(c =>
           {
               c.Validation.EnableDataAnnotationsSupport = true;

               c.Binding.UsePropertyNamingPolicy = true;
               c.Binding.ReflectionCache.AddFromWeb();
               c.Binding.ValueParserFor<Guid>(x => new(Guid.TryParse(x, out var res), res));

               // Prefer AOT/source-generated metadata; configure a deterministic, cycle-free resolver.
               MyResolver.Configure(c.Serializer.Options);

               c.Endpoints.RoutePrefix = "api";
               c.Endpoints.ShortNames = false;
               c.Endpoints.PrefixNameWithFirstTag = true;
               c.Endpoints.Filter = ep => ep.EndpointTags?.Contains("exclude") is not true;

               var isDev = app.Environment.IsDevelopment();
               c.Endpoints.Configurator = ep =>
               {
                   // existing pre-processors and conditional docs tweaks
                   ep.PreProcessors(Order.Before, new AdminHeaderChecker());
                   if (ep.EndpointTags?.Contains("Orders") is true)
                       ep.Description(b => b.Produces<ErrorResponse>(400, "application/problem+json"));

                   // centralized: hide fastendpoints/* routes from docs outside Development
                   if (!isDev)
                   {
                       if (ep.Routes?.Any(r => r.Contains(AppRoutes.fastendpoints_prefix, StringComparison.OrdinalIgnoreCase)) == true)
                           ep.Description(b => b.WithTags("exclude"));
                   }
               };

               c.Versioning.Prefix = "ver";

               c.Throttle.HeaderName = "X-Custom-Throttle-Header";
               c.Throttle.Message = "Custom Error Response";
           })
           .UseEndpoints(c =>
           {
               c.MapGet("test", () => "hello world!").WithTags($"Heading:{Web.Docs.ApiHeadings.MinimalAPI}");
               c.MapGet("test/{testId:int?}", (int? testId) => $"hello {testId}").WithTags($"Heading:{Web.Docs.ApiHeadings.MinimalAPI}");
           });

        // Docs UI (Swagger/Scalar) is now mapped via Web.Features.Docs.DocsFeature.UseDocsUIIfNonProduction in Program.cs
        return app;
    }
}
