namespace Security.Jwt;

public class FE_JWTGetToken : EndpointWithoutRequest<string>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    private readonly Web.Infrastructure.IJwtTokenService _jwt;

    public FE_JWTGetToken(Web.Infrastructure.AppEnv appEnv, Web.Infrastructure.IJwtTokenService jwt)
    {
        _appEnv = appEnv;
        _jwt = jwt;
    }

    public override void Configure()
    {
        Post(AppRoutes.fastendpoints_jwt);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(FE_JWTGetToken);
            s.Description = "Issues a short-lived JWT for demo purposes.";
        });
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
            return Send.NotFoundAsync();

        var token = _jwt.CreateToken(o =>
        {
            // include admin role and email to satisfy policies like AdminRolePolicy/AdminEmailPolicy
            o.User.Roles.Add(Role.Admin);
            o.User.Claims.Add(new System.Security.Claims.Claim(Claim.Email, "admin@email.com"));
        });
        return Send.OkAsync(token);
    }
}