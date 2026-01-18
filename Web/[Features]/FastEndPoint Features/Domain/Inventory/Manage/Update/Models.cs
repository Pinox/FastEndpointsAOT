using FluentValidation;
using Shared.Contracts.Inventory;

namespace Inventory.Manage.Update;

/// <summary>
/// Derived request with Web-specific binding attributes.
/// Only overrides properties that need [From], etc.
/// </summary>
public class Request : UpdateProductRequestBase
{
    [From(Claim.AdminID)]
    public override string? UserID { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required!");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required!");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price is required!");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("ModifiedBy is required!");
    }
}

// Response type moved to Shared.Contracts.Inventory.UpdateProductResponse