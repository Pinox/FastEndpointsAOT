using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Security.Jwt;

// GET /dev/checkjwt -> requires JWT Bearer
public sealed class FE_JWTVerify : EndpointWithoutRequest<string>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public FE_JWTVerify(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.fastendpoints_checkjwt);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(FE_JWTVerify);
            s.Description = "Returns 'pong-jwt' when authorized with a valid Bearer token.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
        {
            await Send.NotFoundAsync();
            return;
        }

        await Send.OkAsync("pong-jwt");
    }
}