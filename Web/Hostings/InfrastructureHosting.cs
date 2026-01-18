using Microsoft.Extensions.DependencyInjection;
using Web.Infrastructure;
using Web.Services;

namespace Web.Hostings;

public static class InfrastructureHosting
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddAntiforgery();
        return services;
    }

    public static IServiceCollection AddRuntimeAotAndEnv(this IServiceCollection services)
    {
        services.AddSingleton<IRequestBinder<EmptyRequest>, RequestBinder<EmptyRequest>>();
        services.AddSingleton<AppEnv>();
        return services;
    }

    public static IApplicationBuilder UseAntiforgeryPipeline(this IApplicationBuilder app)
        => app.UseAntiforgeryFE(additionalContentTypes: ["application/json"]);
}
