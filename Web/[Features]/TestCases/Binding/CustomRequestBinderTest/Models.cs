using System.Text.Json;
using Web.Serialization;

namespace TestCases.CustomRequestBinder;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class Request
{
    public string Id { get; set; }
    public Product? Product { get; set; }
    public string CustomerID { get; set; }
}

public class Binder : RequestBinder<Request>
{
    public override async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken ct)
        => new()
        {
            Id = ctx.HttpContext.Request.RouteValues["id"]?.ToString()!,
            CustomerID = ctx.HttpContext.Request.Headers["CustomerID"].ToString()!,
            Product = await JsonSerializer.DeserializeAsync(
                ctx.HttpContext.Request.Body,
                AppJsonContext.Default.CustomRequestBinderProduct,
                ct)
        };
}

public class Response : Request { }