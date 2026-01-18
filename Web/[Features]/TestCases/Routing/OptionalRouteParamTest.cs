namespace TestCases.Routing;

public class OptionalRouteParamTest : Ep
    .Req<OptionalRouteParamTest.Request>
    .Res<string>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_routing_offer);
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await Send.OkAsync(req.OfferId ?? "default offer");
    }

    public record Request(string? OfferId);
}