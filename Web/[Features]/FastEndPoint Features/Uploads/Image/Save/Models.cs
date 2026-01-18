using FluentValidation;
using Shared.Contracts.Uploads;

namespace Uploads.Image.Save;

/// <summary>
/// Derived request from Shared base.
/// </summary>
public class Request : ImageSaveRequestBase
{
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(i => i.Width)
            .GreaterThan(10).WithMessage("Image width too small")
            .LessThan(2000).WithMessage("Image width is too large");

        RuleFor(i => i.Height)
            .GreaterThan(10).WithMessage("Image height too small")
            .LessThan(2000).WithMessage("Image width is too large");
    }
}
