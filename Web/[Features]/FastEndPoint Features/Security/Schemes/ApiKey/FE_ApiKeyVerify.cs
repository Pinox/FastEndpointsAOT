using Web.Infrastructure;

namespace Security.ApiKey;

// GET /dev/apikey -> requires X-API-Key header
public sealed class FE_ApiKeyVerify : EndpointWithoutRequest<string>
{
    private readonly AppEnv _appEnv;
    public FE_ApiKeyVerify(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.fastendpoints_apikey);
        AuthSchemes(ApiKeyAuth.Scheme);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(FE_ApiKeyVerify);
            s.Description = $"Returns 'pong-apikey' when the '{ApiKeyAuth.HeaderName}' header is valid.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
        {
            await Send.NotFoundAsync();
            return;
        }

        await Send.OkAsync("pong-apikey");
    }
}