namespace Security.ApiKey;

// GET /dev/apikey -> returns the configured API key; only available in Development
public sealed class FE_ApiKeyGet : EndpointWithoutRequest<string>
{
    private readonly IConfiguration _cfg;
    private readonly IHostEnvironment _env;

    public FE_ApiKeyGet(IConfiguration cfg, IHostEnvironment env)
    {
        _cfg = cfg;
        _env = env;
    }

    public override void Configure()
    {
        Get(AppRoutes.fastendpoints_getapikey);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(FE_ApiKeyGet);
            s.Description = "Returns the configured API key. This endpoint is ONLY available when the app runs in Development.";
        });
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        if (!_env.IsDevelopment())
            return Send.NotFoundAsync(); // hide in non-dev

        var key = _cfg["ApiKey"] ?? string.Empty;
        return Send.OkAsync(key);
    }
}