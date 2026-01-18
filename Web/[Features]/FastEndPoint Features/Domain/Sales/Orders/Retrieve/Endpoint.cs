using Web.PipelineBehaviors.PreProcessors;
using Web.Infrastructure;
using Shared.Contracts.Sales.Orders;

namespace Domain.Sales.Orders.Retrieve;

public class Endpoint : Endpoint<RetrieveOrderRequest, RetrieveOrderResponse>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get("/sales/orders/retrieve/{OrderID}");
        PreProcessor<SecurityProcessor<RetrieveOrderRequest>>();
        AllowAnonymous();
        Tags("Orders");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = "Retrieve order";
            s.Description = "Retrieves an order by id.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new RetrieveOrderRequest { TenantID = "TEN-1", OrderID = "123", ContentType = "application/json" };
            }
        });
    }

    public override async Task HandleAsync(RetrieveOrderRequest r, CancellationToken c)
        => await Send.OkAsync(new RetrieveOrderResponse());
}