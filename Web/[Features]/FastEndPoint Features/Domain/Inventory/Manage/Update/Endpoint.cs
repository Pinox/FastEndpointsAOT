namespace Inventory.Manage.Update;

public class Endpoint : Endpoint<Request>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public Endpoint(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes(AppRoutes.inventory_manage_update);
        Policies(PolicyNames.AdminOnly);
        Permissions(
            Allow.Inventory_Create_Item,
            Allow.Inventory_Update_Item);
        AccessControl("Inventory_Update_Item", "Admin");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        Summary(s =>
        {
            s.Summary = nameof(Endpoint);
            s.Description = "Updates an existing inventory item by id.";
            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request
                {
                    UserID = "USR0001",
                    Id = 10,
                    Name = "Apple Juice",
                    Description = "Updated description",
                    Price = 12.99m,
                    QtyOnHand = 25,
                    ModifiedBy = "admin"
                };
            }
        });
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        //this is a test case for checking security policy restrictions
        return Send.OkAsync();
    }
}
