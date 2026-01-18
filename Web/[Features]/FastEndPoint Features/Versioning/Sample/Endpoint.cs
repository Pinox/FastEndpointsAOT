using Shared.Contracts.Versioning;

namespace Versioning.Sample;

public class EndpointV1 : Endpoint<VersionSampleRequest, object>
{
    public override void Configure()
    {
        Get(AppRoutes.samples_versioning_item);
        Version(1);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Versioning}"));
        Summary(s =>
        {
            s.Summary = "Versioning sample (v1)";
            s.Description = "Returns a payload with version =1.";
        });
    }

    public override async Task HandleAsync(VersionSampleRequest r, CancellationToken ct)
        => await Send.OkAsync(new { Version = 1, r.Id });
}

public class EndpointV2 : Endpoint<VersionSampleRequest, object>
{
    public override void Configure()
    {
        Get(AppRoutes.samples_versioning_item);
        Version(2);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Versioning}"));
        Summary(s =>
        {
            s.Summary = "Versioning sample (v2)";
            s.Description = "Returns a payload with version =2.";
        });
    }

    public override async Task HandleAsync(VersionSampleRequest r, CancellationToken ct)
        => await Send.OkAsync(new { Version = 2, r.Id });
}
