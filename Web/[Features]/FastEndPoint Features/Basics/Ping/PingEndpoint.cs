using Web.Infrastructure;

namespace Basics.Ping;

public class PingEndpoint : EndpointWithoutRequest<string>
{
    private readonly AppEnv _appEnv;
    public PingEndpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.basics_ping);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Basics}"));
        Summary(s =>
        {
            s.Summary = nameof(PingEndpoint);
            s.Description = "Basic health check endpoint that returns 'pong'.";
            if (_appEnv.IsDevelopment)
            {
                // no request body, so no example request
            }
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
        => await Send.OkAsync("pong");
}
