using System.ComponentModel;
using Web.Infrastructure;
using Shared.Contracts.Customers;

namespace Domain.Customers.Update;

/// <summary>
/// Derived request with Web-specific binding attributes.
/// Only overrides properties that need [FromClaim], [QueryParam], etc.
/// </summary>
public class Request : UpdateCustomerRequestBase
{
    [FromClaim(Claim.CustomerID, IsRequired = false)] //allow non customers to set the customer id for updates
    [DefaultValue("test default val")]
    public override string CustomerID { get; set; } = "";

    [QueryParam, DefaultValue("query test default val")]
    public override string Name { get; set; } = "";
}

public class Endpoint : Endpoint<Request>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes(
            AppRoutes.customer_update,
            AppRoutes.customer_save);
        Claims(
            Claim.AdminID,
            Claim.CustomerID);
        PermissionsAll(
            Allow.Customers_Create,
            Allow.Customers_Update,
            Allow.Customers_Retrieve);
        AccessControl("Customers_Update", "Admin");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = "Update customer";
            s.Description = "Updates the customer record for the given customer id.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request
                {
                    CustomerID = "CST001",
                    Name = "Jane Doe",
                    Age = 30,
                    Address = "123 Main St"
                };
            }
        });
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!User.HasPermission(Allow.Customers_Update))
            ThrowError("no permission!");

        return Send.OkAsync(req.CustomerID);
    }
}