using Web.Infrastructure;

namespace Uploads.Image.SaveTyped;

public class Endpoint : Endpoint<Request>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Verbs(Http.POST, Http.PUT);
        AllowAnonymous(Http.POST);
        Routes(AppRoutes.uploads_image_save_typed);
        AccessControl("Image_Update");
        Permissions(Allow.Image_Update);
        Claims(Claim.AdminID);
        AllowFileUploads();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Uploads}"));
        Options(
            b => b.Produces<byte[]>(200, "image/png", "test/image")
                  .Produces<string>(204, "text/plain", "test/notcontent"));
        Summary(s =>
        {
            s.Summary = nameof(Endpoint);
            s.Description = "Uploads typed files and demonstrates streaming a file in the response.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request { ID = "IMG-2", Width =800, Height =600, GuidId = Guid.Empty };
            }
        });
    }

    public override async Task HandleAsync(Request r, CancellationToken ct)
    {
        if (r.File1.Length >0 && r.File2.Length >0 && r.File3?.Length >0)
        {
            await Send.StreamAsync(r.File1.OpenReadStream(), "test.png", r.File1.Length, "image/png", cancellation: ct);

            return;
        }

        await Send.NoContentAsync();
    }
}