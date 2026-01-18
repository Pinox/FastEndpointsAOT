using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Hostings;

public static class DevelopmentHosting
{
    // Placeholder for development-only services (wired only in Development)
    public static IServiceCollection AddDevelopmentOnly(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            // Add dev-only services here if/when needed
        }
        return services;
    }

    // Placeholder for development-only pipeline (wired only in Development)
    public static IApplicationBuilder UseDevelopmentOnly(this IApplicationBuilder app)
    {
        var web = (WebApplication)app;
        if (web.Environment.IsDevelopment())
        {
            // Add dev-only middleware/endpoints here if/when needed
        }
        return app;
    }
}
