using Microsoft.AspNetCore.Authorization;

namespace TestCases.EmptyRequestTest;

[
    HttpGet(AppRoutes.testcases_empty_request_test),
    AllowAnonymous
]
public class EmptyRequestEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        => await Send.OkAsync(ct);
}