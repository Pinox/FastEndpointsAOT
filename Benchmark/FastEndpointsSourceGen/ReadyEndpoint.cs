using FastEndpoints;

namespace FEBenchSourceGen;

/// <summary>
/// Health check endpoint for accurate startup time measurement.
/// Returns immediately with no request processing overhead.
/// </summary>
public class ReadyEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/ready");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return HttpContext.Response.SendOkAsync(ct);
    }
}
