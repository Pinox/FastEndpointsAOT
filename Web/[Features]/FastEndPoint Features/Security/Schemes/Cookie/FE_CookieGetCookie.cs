using Microsoft.AspNetCore.Authentication;

namespace Security.Cookie;

// POST /dev/cookie/login -> sets an auth cookie for testing
public sealed class FE_CookieGetCookie : EndpointWithoutRequest
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public FE_CookieGetCookie(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Post(AppRoutes.fastendpoints_cookie_login);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s => s.Summary = nameof(FE_CookieGetCookie));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
        {
            await Send.NotFoundAsync();
            return;
        }

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "cookie-user"),
            // include email claim consistent with Web.Auth.Claim.Email
            new System.Security.Claims.Claim(Claim.Email, "admin@email.com"),
            // include an admin role claim using the standard role claim type so role checks work
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, Role.Admin)
        };

        var identity = new System.Security.Claims.ClaimsIdentity(
            claims,
            "Cookies",
            System.Security.Claims.ClaimTypes.Name,
            System.Security.Claims.ClaimTypes.Role);

        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        // rely on global cookie settings; no per-endpoint expiry/persistence here
        var props = new AuthenticationProperties();
        var authService = HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
        await authService.SignInAsync(HttpContext, "Cookies", principal, props);
        await Send.OkAsync();
    }
}