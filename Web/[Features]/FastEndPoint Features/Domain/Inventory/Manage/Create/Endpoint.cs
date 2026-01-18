using Claim = Web.Auth.Claim;
using Web.Infrastructure;
using Shared.Contracts.Inventory;

namespace Domain.Inventory.Manage.Create;

public class Endpoint : Endpoint<Request>
{
    private readonly AppEnv _appEnv;
    public Endpoint(AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Post(AppRoutes.inventory_manage_create);
        Policies(PolicyNames.AdminOnly);
        Permissions(
            Allow.Inventory_Create_Item,
            Allow.Inventory_Update_Item);
        AccessControl("Inventory_Create_Item", "Admin");
        ClaimsAll(
            Claim.AdminID,
            "test-claim");
        Policy(b => b.RequireRole(Role.Admin));
        Description(
            x => x.Accepts<Request>("application/json")
                  .Produces(201)
                  .Produces(500)
                  .WithTags($"Heading:{ApiHeadings.Domain}")
                  .WithName("CreateInventoryItem"),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = nameof(Endpoint);
            s.Description = "Creates a new inventory item and publishes a domain event.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request
                {
                    Description = "first description",
                    Price = 10,
                    QtyOnHand = 10,
                    Id = 1,
                    Name = "first name",
                    Username = "blah"
                };
            }
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var validation = ValidationContext<Request>.Instance;

        if (string.IsNullOrEmpty(req.Description))
            AddError(x => x.Description, "Please enter a product descriptions!");

        if (req.Price > 1000)
        {
            //AddError(x => x.Price, "Price is too high!");
            validation.AddError(x => x.Price, "Price is too high!");
        }

        // Example: raise domain event for inventory updates in production system
        // Publish an internal system event, handled by production handlers
        var eventdto = new Web.SystemEvents.NewOrderCreated
        {
            CustomerName = req.Name,
            OrderID = 0,
            OrderTotal = req.Price
        };
        await PublishAsync(eventdto, Mode.WaitForNone, ct);

        ThrowIfAnyErrors();

        if (req.Name == "Apple Juice")
        {
            //ThrowError("Product already exists!");
            validation.ThrowError("Product already exists!");
        }

        var res = new CreateProductResponse
        {
            ProductId = new Random().Next(1, 1000),
            ProductName = req.Name
        };

        await Send.CreatedAtAsync<Domain.Inventory.GetProduct.Endpoint>(
            routeValues: new { ProductID = res.ProductId },
            responseBody: res,
            generateAbsoluteUrl: req.GenerateFullUrl);
    }
}