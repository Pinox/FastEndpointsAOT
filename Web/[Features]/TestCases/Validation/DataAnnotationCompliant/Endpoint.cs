using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestCases.DataAnnotationCompliant;

public class Request
{
    /// <summary>
    /// id of the plain text request
    /// </summary>
    [Description("CurrentId"), DeniedValues(10)]
    public int Id { get; init; }

    [Required, StringLength(10, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    public NestedRequest Meta { get; set; }

    public List<ChildRequest> Children { get; set; } = new();
}

public class NestedRequest
{
    [Required, StringLength(10, MinimumLength = 1)]
    public string Gender { get; set; }

    [Range(1, 150)]
    public int Age { get; set; }
}

public class ChildRequest
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; set; }

    [Range(0, 100)]
    public int Age { get; set; }

    [Required, StringLength(10, MinimumLength = 1)]
    public string Gender { get; set; }
}

[Obsolete("Obsolete", false)]
public class Endpoint : Endpoint<Request, Request>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_data_annotation);
        AllowAnonymous();
        Description(b => b.WithTags("Hide"));
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
        => await Send.OkAsync(r);
}