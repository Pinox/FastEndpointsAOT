using Shared.Contracts.Binding;

namespace Binding.RouteAndQuery;

public class Endpoint : Endpoint<RouteAndQueryRequest>
{
    public override void Configure()
    {
        Get(AppRoutes.samples_binding_items);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Binding}"));
        Summary(s => s.Description = "Demonstrates route and query param binding.");
    }

    public override async Task HandleAsync(RouteAndQueryRequest r, CancellationToken ct)
        => await Send.OkAsync(new { r.Id, r.Page, r.PageSize });
}
