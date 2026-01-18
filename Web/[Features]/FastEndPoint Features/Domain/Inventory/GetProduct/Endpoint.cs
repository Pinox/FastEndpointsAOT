using Shared.Contracts.Inventory;

namespace Domain.Inventory.GetProduct;

public class Endpoint : EndpointWithoutRequest<GetProductResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes(AppRoutes.inventory_get_product);
        AccessControl("Inventory_Retrieve_Item", "Admin");
        AllowAnonymous();
        ResponseCache(10);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(x =>
        {
            x.Summary = nameof(Endpoint);
            x.Description = "Returns product metadata for the given product id.";
            x.ResponseParam<GetProductResponse>(r => r.LastModified, "UTC ticks of last modification");
        });
    }

    public override Task<GetProductResponse> ExecuteAsync(CancellationToken ct)
        => Task.FromResult(
            new GetProductResponse
            {
                LastModified = DateTime.UtcNow.Ticks,
                ProductID = Route<string>("ProductID")
            });
}