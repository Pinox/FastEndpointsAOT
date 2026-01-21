namespace TestCases.KeyedServicesTests;

public interface IKeyedService
{
    string KeyName { get; init; }
}

public sealed class MyKeyedService(string keyName) : IKeyedService
{
    public string KeyName { get; init; } = keyName;
}

sealed class Endpoint : EndpointWithoutRequest<string>
{
    [KeyedService("AAA")]
    public IKeyedService KeyedService { get; set; }

    public override void Configure()
    {
        Get(AppRoutes.testcases_keyed_services_test);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
        => await Send.OkAsync(KeyedService.KeyName);
}