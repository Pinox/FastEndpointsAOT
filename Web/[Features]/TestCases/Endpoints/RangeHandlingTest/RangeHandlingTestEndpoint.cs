using System.Text;

namespace TestCases.RangeHandlingTest;

public class Endpoint : EndpointWithoutRequest
{
    static readonly byte[] content = Encoding.UTF8.GetBytes("abcdefghijklmnopqwstuvwxyz");

    public override void Configure()
    {
        Get(AppRoutes.testcases_range);
        AllowAnonymous();
        Description(b => b.Accepts<Request>());
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.BytesAsync(content, contentType: "text/plain", enableRangeProcessing: true);
    }
}