using Web.Infrastructure;

namespace Uploads.Image.Save;

public class Endpoint : Endpoint<Request>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Verbs(Http.POST, Http.PUT);
        AllowAnonymous(Http.POST);
        Routes(AppRoutes.uploads_image_save);
        Permissions(Allow.Image_Update);
        Claims(Claim.AdminID);
        AllowFileUploads();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Uploads}"));
        Options(b => b.Accepts<Request>("multipart/form-data"));
        Summary(s =>
        {
            s.Summary = nameof(Endpoint);
            s.Description = "Uploads an image and returns the file back in the response for demo.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request { ID = "IMG-1", Width = 640, Height = 480 };
            }
        });
    }

    public override Task HandleAsync(Request r, CancellationToken ct)
    {
        if (Files.Count > 0)
        {
            var file = Files[0];

            return Send.StreamAsync(file.OpenReadStream(), "test.png", file.Length, "image/png");
        }

        return Send.NoContentAsync();
    }
}