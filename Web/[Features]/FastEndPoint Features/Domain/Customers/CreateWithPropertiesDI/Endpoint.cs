using Web.Services;
using Shared.Contracts.Customers;

namespace Domain.Customers.CreateWithPropertiesDI;

/// <summary>
/// Derived request with Web-specific binding attributes.
/// Only overrides properties that need [From], [HasPermission], etc.
/// </summary>
public class Request : CreateCustomerRequestBase
{
    /// <summary>
    /// Route parameter for customer ID
    /// </summary>
    public string cID { get; set; } = "";

    [From(Claim.Email)]
    public override string? CreatedBy { get; set; }

    [HasPermission(Allow.Customers_Create)]
    public override bool HasCreatePermission { get; set; }
}

public class Endpoint : Endpoint<Request>
{
    public required IEmailService Emailer { get; init; }

    public override void Configure()
    {
        Verbs(Http.POST, Http.GET);
        Routes(
            "/customer/new/2{RefererID}",
            "/customer/{cID}/new2/{SourceID}",
            AppRoutes.customer_save2);
        AccessControl("Customers_Create_2");
        DontAutoTag();
        Description(x => x.WithTags($"Heading:{ApiHeadings.Domain}"));
    }

    public override async Task HandleAsync(Request r, CancellationToken t)
    {
        Logger.LogInformation("customer creation has begun!");

        if (r.PhoneNumbers?.Count() < 2)
            ThrowError("Not enough phone numbers!");

        var msg = Emailer.SendEmail() + " " + r.CreatedBy;

        await Send.OkAsync(msg ?? "emailer not resolved!");
    }
}