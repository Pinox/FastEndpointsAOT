using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Auth;
using Web.Infrastructure;

namespace Web.Hostings;

public static class AuthHosting
{
    // Services: AuthN/AuthZ + supporting settings and token service
    public static IServiceCollection AddAuthNAuthZServices(this IServiceCollection services, IConfiguration config)
    {
        var authSettings = config.GetSection("Auth").Get<AuthSettings>() ?? new AuthSettings();
        services.AddSingleton(authSettings);

        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                // Fallback key for development/testing when appsettings.json is not present
                var tokenKey = config["TokenKey"] ?? "default-dev-signing-key-minimum-32-chars";
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(tokenKey)),
                    NameClaimType = "name",
                    RoleClaimType = "role",
                    ValidateLifetime = true
                };
                o.MapInboundClaims = false;
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuth.Scheme, _ => { })
            .AddCookie("Cookies", o =>
            {
                o.Cookie.Name = authSettings.Cookies.Name;
                o.SlidingExpiration = authSettings.Cookies.SlidingExpiration;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(authSettings.Cookies.ExpiresMinutes);
                o.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnSigningIn = ctx =>
                    {
                        ctx.Properties.IsPersistent = authSettings.Cookies.IsPersistent;
                        if (authSettings.Cookies.ExpiresMinutes > 0)
                            ctx.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(authSettings.Cookies.ExpiresMinutes);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(o =>
        {
            o.AddPolicy(PolicyNames.AdminRole, b => b.RequireRole(Role.Admin));
            o.AddPolicy(PolicyNames.AdminOnly, b => b.RequireRole(Role.Admin));
            o.AddPolicy(PolicyNames.AdminEmail, p => p.RequireClaim(Claim.Email, "admin@email.com"));
            o.AddPolicy(PolicyNames.User, p => p.RequireAuthenticatedUser());
        });

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }

    // Pipeline: Use auth and authorization
    public static IApplicationBuilder UseAuthNAuthZPipeline(this IApplicationBuilder app)
        => app.UseAuthentication().UseAuthorization();
}
