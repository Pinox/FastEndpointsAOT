using Web.Infrastructure;
using Shared.Contracts.Uploads;

namespace Binding.Multipart;

/// <summary>
/// Derived request adding server-only IFormFile and [FromForm] attributes.
/// </summary>
public class UploadRequest : MultipartUploadRequestBase
{
    [FromForm]
    public override string? Title { get; set; }

    [FromForm]
    public override string? Description { get; set; }

    /// <summary>
    /// Server-only: actual file from multipart form
    /// </summary>
    public IFormFile? File { get; set; }
}

public class Endpoint : Endpoint<UploadRequest>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Post(AppRoutes.samples_binding_multipart_upload);
        AllowAnonymous();
        AllowFileUploads();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Binding}"));
        Options(b => b.Accepts<UploadRequest>("multipart/form-data"));
        Summary(s =>
        {
            s.Summary = "Multipart upload";
            s.Description = "Accepts multipart/form-data with form fields and one file.";
            if (_appEnv.IsDevelopment)
                s.ExampleRequest = new UploadRequest { Title = "Sample", Description = "Test file" };
        });
    }

    public override async Task HandleAsync(UploadRequest r, CancellationToken ct)
    {
        if (r.File is null || r.File.Length == 0)
        {
            await Send.OkAsync(new { error = "file is required" }, ct);
            return;
        }

        // In a real app, stream to disk/cloud. Here we just echo metadata.
        var file = r.File;
        var result = new
        {
            r.Title,
            r.Description,
            file.FileName,
            file.ContentType,
            file.Length
        };

        await Send.OkAsync(result, ct);
    }
}
