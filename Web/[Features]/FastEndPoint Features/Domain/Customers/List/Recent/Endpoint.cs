using Shared.Contracts.Customers;

namespace Domain.Customers.List.Recent;

public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes(AppRoutes.customer_list_recent);
        Policies(PolicyNames.AdminOnly);
        Roles(
            Role.Admin,
            Role.Staff);
        Permissions(
            Allow.Customers_Retrieve,
            Allow.Customers_Create);
        AccessControl("Customers_Retrieve", "Admin");
        Options(o => o.Produces<ListRecentCustomersResponse>());
        Version(0, deprecateAt: 1);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = "List recent customers";
            s.Description = "Returns a list of recent customers with their ids.";
        });
    }

    public override Task<object?> ExecuteAsync(CancellationToken ct)
        => Task.FromResult(
            (object?)new ListRecentCustomersResponse
            {
                Customers =
                [
                    new("ryan gunner", 123),
                    new("debby ryan", 124),
                    new("ryan reynolds", 321)
                ]
            });
}

// Response class moved to Shared.Contracts.Customers.ListRecentCustomersResponse

public class Endpoint_V1 : Endpoint
{
    public override void Configure()
    {
        base.Configure();
        Version(1, deprecateAt: 2);
        AuthSchemes("ApiKey", "Cookies");
    }
}