using Microsoft.AspNetCore.Builder;

namespace Web.Hostings;

public static class EndpointsHosting
{
    // Conventional test endpoints kept for parity with original setup
    public static IApplicationBuilder MapAdditionalEndpoints(this IApplicationBuilder app)
    {
        var web = (WebApplication)app;
        web.UseEndpoints(c =>
        {
            c.MapGet("test", () => "hello world!").WithTags($"Heading:{Web.Docs.ApiHeadings.Hostings}");
            c.MapGet("test/{testId:int?}", (int? testId) => $"hello {testId}").WithTags($"Heading:{Web.Docs.ApiHeadings.Hostings}");
        });
        return app;
    }
}
