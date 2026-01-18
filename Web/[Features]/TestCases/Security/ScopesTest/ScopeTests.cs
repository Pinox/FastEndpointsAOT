namespace TestCases.ScopesTest;

public class ScopeTestAnyPassEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_scope_tests_any_scope_pass);
        Scopes("two", "three", "blah");
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync("ok!");
    }
}

public class ScopeTestAnyFailEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_scope_tests_any_scope_fail);
        Scopes("nine");
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync("ok!");
    }
}

public class ScopeTestAllPassEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_scope_tests_all_scope_pass);
        ScopesAll("one", "two", "three");
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync("ok!");
    }
}

public class ScopeTestAllFailEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_scope_tests_all_scope_fail);
        ScopesAll("one", "two", "three", "blah");
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync("ok!");
    }
}