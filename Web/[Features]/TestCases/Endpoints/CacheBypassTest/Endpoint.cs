namespace TestCases.Endpoints.CacheBypassTest;

sealed class Request
{
    public Guid Id { get; set; }
}

sealed class CachedResponseEndpoint : Endpoint<Request, Guid>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_response_cache_bypass_test);
        AllowAnonymous();
        ResponseCache(60);
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        await Send.OkAsync(r.Id);
    }
}

sealed class CachedOutputEndpoint : Endpoint<Request, Guid>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_output_cache_bypass_test);
        AllowAnonymous();
        Options(b => b.CacheOutput());
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        var q = HttpContext.Request.Query;
        await Send.OkAsync(r.Id);
    }
}