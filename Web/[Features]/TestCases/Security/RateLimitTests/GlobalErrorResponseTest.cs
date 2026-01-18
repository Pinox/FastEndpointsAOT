namespace TestCases.RateLimitTests;

public class GlobalErrorResponseTest : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_global_throttle_error_response);
        AllowAnonymous();
        Summary(s => s.Params["OtherID"] = "the description for other id");
        Throttle(3, 120);
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(CancellationToken ct)
        => await Send.OkAsync(
            new()
            {
                CustomerID = Query<int>("CustomerID", isRequired: false),
                OtherID = Query<int>("OtherID", isRequired: false)
            },
            cancellation: ct);
}