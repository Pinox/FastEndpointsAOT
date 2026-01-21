using Microsoft.Extensions.DependencyInjection;
using TestCases.KeyedServicesTests;
using TestCases.UnitTestConcurrencyTest;
using Web;
using Web.Infrastructure;
using Web.Services;

namespace Web.Hostings;

public static class InfrastructureHosting
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddAntiforgery();
        
        // Source-generated service registrations for [RegisterService<T>] attributes
        services.RegisterServicesFromWeb();
        
        // Keyed services for TestCases.KeyedServicesTests
        services.AddKeyedTransient<IKeyedService>("AAA", (_, _) => new MyKeyedService("AAA"));
        services.AddKeyedTransient<IKeyedService>("BBB", (_, _) => new MyKeyedService("BBB"));
        
        // Singleton for TestCases.UnitTestConcurrencyTest
        services.AddSingleton(new SingltonSVC(0));
        
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
