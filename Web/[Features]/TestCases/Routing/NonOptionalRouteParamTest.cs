namespace TestCases.Routing;

public class NonOptionalRouteParamTest : Ep
    .Req<NonOptionalRouteParamTest.Request>
    .Res<string>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_routing_user);
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await Send.OkAsync(req.UserId);
    }

    public record Request(string UserId);
}