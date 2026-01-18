namespace TestCases.MissingClaimTest;

public class DontThrowIfMissingEndpoint : Endpoint<DontThrowIfMissingRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes(AppRoutes.testcases_missing_claim_test_dont_throw);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(DontThrowIfMissingRequest req, CancellationToken ct)
    {
        return Send.OkAsync($"you sent {req.TestProp}");
    }
}
