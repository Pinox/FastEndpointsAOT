using FastEndpoints.Security;

namespace Web.Hostings;

public static class SecurityUtilitiesHosting
{
    public static IApplicationBuilder UseJwtRevocationPipeline(this IApplicationBuilder app)
        => app.UseJwtRevocation<JwtBlacklistChecker>();
}
