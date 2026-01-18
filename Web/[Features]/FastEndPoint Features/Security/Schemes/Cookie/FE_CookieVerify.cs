namespace Security.Cookie;

// GET /dev/cookie -> requires auth cookie
public sealed class FE_CookieVerify : EndpointWithoutRequest<string>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public FE_CookieVerify(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.fastendpoints_cookie);
        AuthSchemes("Cookies");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(FE_CookieVerify);
            s.Description = "Returns 'pong-cookie' when authenticated via cookie (scheme 'Cookies').";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
        {
            await Send.NotFoundAsync();
            return;
        }

        await Send.OkAsync("pong-cookie");
    }
}