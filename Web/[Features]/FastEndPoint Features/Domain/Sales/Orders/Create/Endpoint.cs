using Web.PipelineBehaviors.PostProcessors;
using Web.PipelineBehaviors.PreProcessors;
using Web.Services;
using Web.SystemEvents;
using Web.Infrastructure;
using Shared.Contracts.Sales.Orders;

namespace Domain.Sales.Orders.Create;

public class Endpoint : Endpoint<CreateOrderRequest, CreateOrderResponse, MyMapper>
{
    public IEmailService Emailer { get; set; }
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Post("/sales/orders/create/{guidTest}", "/sales/orders/create");
        AccessControl( //Allows creation of orders by anybody who has this permission code.
            "Sales_Order_Create",
            Apply.ToThisEndpoint);
        PreProcessors(new MyRequestLogger<CreateOrderRequest>());
        PostProcessors(new MyResponseLogger<CreateOrderRequest, CreateOrderResponse>());
        Tags("Orders");
        Description(b => b.WithDisplayName(nameof(Endpoint)).WithTags($"Heading:{Web.Docs.ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = "Create order";
            s.Description = "Creates a sales order and publishes a notification.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new CreateOrderRequest { CustomerID =1, ProductID =2, Quantity =3, GuidTest = Guid.Empty };
            }
        });
    }

    public override async Task HandleAsync(CreateOrderRequest r, CancellationToken t)
    {
        var fullName = "x y"; // simplified for production sample
        var userType = User.ClaimValue(Claim.UserType);

        var saleNotification = new NewOrderCreated
        {
            CustomerName = $"new customer ({fullName}) ({userType})",
            OrderID = Random.Shared.Next(0,10000),
            OrderTotal =12345.67m
        };

        await PublishAsync(saleNotification, Mode.WaitForNone, t);

        await Send.OkAsync(
            new CreateOrderResponse
            {
                Message = "order created!",
                AnotherMsg = Map.ToEntity(r),
                OrderID =54321,
                GuidTest = r.GuidTest,
                Header1 =12345,
                Header2 = new(2020,11,12)
            });
    }
}

public class MyMapper : Mapper<CreateOrderRequest, CreateOrderResponse, string>
{
    public override string ToEntity(CreateOrderRequest r)
    {
        var x = Resolve<IEmailService>();

        return x.SendEmail();
    }
}