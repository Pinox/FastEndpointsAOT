using FluentValidation;
using Shared.Contracts.Uploads;

namespace Uploads.Image.SaveTyped;

/// <summary>
/// Derived request from Shared base with server-only IFormFile properties.
/// </summary>
public class Request : ImageSaveTypedRequestBase
{
    // Server-only properties (IFormFile not available in Shared)
    public IFormFile File1 { get; set; }
    public IFormFile File2 { get; set; }
    public IFormFile? File3 { get; set; }
    public IFormFile? File4 { get; set; }

    public IEnumerable<IFormFile> Cars { get; set; }
    public IFormFileCollection Jets { get; set; }
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