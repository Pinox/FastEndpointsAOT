using Web.Services;
using Web.Infrastructure;
using Shared.Contracts.Customers;

namespace Domain.Customers.Create;

/// <summary>
/// Derived request with Web-specific binding attributes.
/// Only overrides properties that need [From], [HasPermission], etc.
/// </summary>
public class Request : CreateCustomerRequestBase
{
    /// <summary>
    /// Route parameter for customer ID (bound from route /customer/{cID}/new/{SourceID})
    /// </summary>
    public string cID { get; set; } = "";

    [From(Claim.Email)]
    public override string? CreatedBy { get; set; }

    [HasPermission(Allow.Customers_Create)]
    public override bool HasCreatePermission { get; set; }
}

public class Endpoint : Endpoint<Request>
{
    readonly IEmailService? _emailer;
    private readonly AppEnv _appEnv;

    public Endpoint(IEmailService emailer, AppEnv appEnv)
    {
        _emailer = emailer;
        _appEnv = appEnv;
    }

    public override void Configure()
    {
        Verbs(Http.POST, Http.GET);
        Routes(
            "/customer/new/{RefererID}",
            "/customer/{cID}/new/{SourceID}",
            AppRoutes.customer_save);
        Permissions(Allow.Customers_Create);
        AccessControl( // Permission for creating a new customer in the system.
            "Customers+Create",
            Apply.ToThisEndpoint,
            "Admin",
            "Manager");
        DontAutoTag();
        Description(x => x.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = nameof(Endpoint);
            s.Description = "Creates a new customer with contact info.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request
                {
                    cID = "CUST-123",
                    CreatedBy = "admin",
                    CustomerName = "Acme Corp",
                    PhoneNumbers = new[] { "+1-555-1234", "+1-555-5678" },
                    HasCreatePermission = true
                };
            }
        });
    }

    public override async Task HandleAsync(Request r, CancellationToken t)
    {
        Logger.LogInformation("customer creation has begun!");

        if (r.PhoneNumbers?.Count() < 2)
            ThrowError("Not enough phone numbers!");

        var msg = _emailer?.SendEmail() + " " + r.CreatedBy;

        await Send.OkAsync(msg ?? "emailer not resolved!");
    }
}
