using Shared.Contracts.Customers;
using Web.Infrastructure;

namespace Customers.UpdateWithHeader;

public class Endpoint : Endpoint<UpdateCustomerWithHeaderRequest>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes(AppRoutes.customer_update_with_header);
        Claims(
            Claim.AdminID,
            Claim.CustomerID);
        Permissions(
            Allow.Customers_Update);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = "Update customer (headers)";
            s.Description = "Updates customer using values bound from headers and body.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new UpdateCustomerWithHeaderRequest(1, "TENANT-1", "Jane Doe",30, "123 Main St");
            }
        });
    }

    public override Task HandleAsync(UpdateCustomerWithHeaderRequest req, CancellationToken ct)
    {
        if (!User.HasPermission(Allow.Customers_Update))
            ThrowError("no permission!");

        return Send.OkAsync(req.TenantID + "|" + req.CustomerID);
    }
}