using FastEndpoints;
using Web.Infrastructure;

namespace Customers.Login;

public class Endpoint : EndpointWithoutRequest
{
    private readonly IJwtTokenService _jwt;

    public Endpoint(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    public class Endpoint_V1 : Endpoint
    {
        public Endpoint_V1(IJwtTokenService jwt)
            : base(jwt)
        { }

        public override void Configure()
        {
            base.Configure();
            Throttle(5, 5);
            Version(1, deprecateAt: 2);
            Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
        }
    }

    public class Endpoint_V2 : Endpoint<EmptyRequest, object>
    {
        public override void Configure()
        {
            Get(AppRoutes.customer_login);
            Version(2);
            AllowAnonymous();
            Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
            Summary(s =>
            {
                s.Summary = "Customer login (v2)";
                s.Description = "Returns 2 as a demo response for v2.";
            });
        }

        public override async Task HandleAsync(EmptyRequest r, CancellationToken ct)
            => await Send.OkAsync(2);
    }

    public override void Configure()
    {
#if DEBUG
        Verbs(Http.GET, Http.POST);
#else
        Verbs(Http.POST);
#endif
        Routes(AppRoutes.customer_login);
        AllowAnonymous();
        Options(b => b.RequireCors(p => p.AllowAnyOrigin()));
        Description(
            b =>
            {
                b.Produces<string>(200, "text/plain")
                 .Produces(400)
                 .Produces(403);
            },
        clearDefaults: true);

        Summary(s =>
        {
            s.Summary = "Customer login";
            s.Description = "Issues a JWT token for the demo customer account.";
        });

        Version(0, 1);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
    }

    public override Task HandleAsync(CancellationToken t)
    {
        var token = _jwt.CreateToken(o =>
        {
            o.User.Permissions.AddRange(
            [
                Allow.Customers_Create,
                Allow.Customers_Update,
                Allow.Customers_Retrieve,
                Allow.Sales_Order_Create
            ]);
            o.User.Roles.Add(Role.Customer);
            o.User[Claim.CustomerID] = "CST001";
            o.User["scope"] = "one two three";
        });

        return Send.OkAsync(token);
    }
}