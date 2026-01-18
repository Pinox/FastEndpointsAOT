namespace TestCases.MissingHeaderTest;

public class ThrowIfMissingEndpoint : Endpoint<ThrowIfMissingRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes(AppRoutes.testcases_missing_header_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(ThrowIfMissingRequest req, CancellationToken ct)
    {
        //this line will never be reached as ErrorResponse will be sent due to claim missing
        return Task.CompletedTask;
    }
}
