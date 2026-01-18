namespace Inventory.Manage.Delete;

public class Endpoint : Endpoint<Request>
{
 private readonly Web.Infrastructure.AppEnv _appEnv;
 public Endpoint(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

 public override void Configure()
 {
 Delete("inventory/manage/delete/{itemID}");
 AccessControl("Inventory_Delete_Item", "Admin");
 AllowAnonymous();
 Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
 Summary(s =>
 {
 s.Summary = nameof(Endpoint);
 s.Description = "Deletes an inventory item by id.";
 if (_appEnv.IsDevelopment)
 {
 s.ExampleRequest = new Request { ItemID = "ITM-1" };
 }
 });
 }

 public override Task HandleAsync(Request r, CancellationToken c)
 {
 return Send.OkAsync();
 }
}