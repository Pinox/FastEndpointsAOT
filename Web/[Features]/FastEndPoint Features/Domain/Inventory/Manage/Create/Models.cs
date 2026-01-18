using FluentValidation;
using Shared.Contracts.Inventory;

namespace Domain.Inventory.Manage.Create;

/// <summary>
/// Derived request with Web-specific binding attributes.
/// Only overrides properties that need [From], [FromClaim], etc.
/// </summary>
public class Request : CreateProductRequestBase
{
    [From(Claim.AdminID)]
    public override string? UserID { get; set; }

    [FromClaim]
    public override string? Username { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required!");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price is required!");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("ModifiedBy is required!");
        RuleFor(x => x.Description)
            .NotEmpty().When(x => false); //due to having a condition, swagger spec should treat this property as not required.
    }
}

// Response class moved to Shared.Contracts.Inventory.CreateProductResponse