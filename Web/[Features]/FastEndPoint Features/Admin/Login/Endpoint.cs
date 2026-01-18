using Web.Services;
using _Claim = System.Security.Claims.Claim;
using Web.Infrastructure;
using Shared.Contracts.Admin;

namespace Admin.Login;

public class Endpoint_V1 : Endpoint<Request, LoginResponse>
{
    private readonly AppEnv _appEnv;
    private readonly IJwtTokenService _jwt;

    public Endpoint_V1(ILogger<Endpoint_V1> logger, IEmailService emailService, AppEnv appEnv, IJwtTokenService jwt)
    {
        _appEnv = appEnv;
        _jwt = jwt;
        logger.LogInformation("constructor injection works!");
        _ = emailService.SendEmail();
    }

    public override void Configure()
    {
        Post(AppRoutes.admin_login);
        RequestBinder(new RequestBinder<Request>(BindingSource.JsonBody));
        AllowAnonymous();
        Options(b => b.RequireCors(p => p.AllowAnyOrigin()));
        Description(
            b =>
            {
                b.Accepts<Request>("application/json");
                b.Produces<LoginResponse>(200, "application/json")
                  .Produces(400)
                  .Produces(403);
            },
        clearDefaults: true);

        Summary(s =>
        {
            s.Summary = "Admin login";
            s.Description = "FE_JWTGetToken for admin user login that returns a JWT token upon successful authentication.";

            if (_appEnv.IsDevelopment)
            {
                s.ExampleRequest = new Request { email = "admin@email.com", Password = "pass" };
            }
        });

        // Only apply throttling in Production to avoid test failures
        if (_appEnv.IsProduction)
        {
            Throttle(5, 5);
        }
        Version(1, deprecateAt: 2);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Admin}"));
    }

    public override async Task HandleAsync(Request r, CancellationToken ct)
    {
        if (r.email == "admin@email.com" && r.Password == "pass")
        {
            var expiryDate = _jwt.GetDefaultExpiryUtcFromNow();

            var userPermissions = Allow.Admin;

            var userClaims = new _Claim[]
            {
                new(Claim.Email, r.email),
                new(Claim.UserType, Role.Admin),
                new(Claim.AdminID, "USR0001"),
                new("test-claim", "test claim val"),
                new("Username", "admin-user")
            };

            var userRoles = new[]
            {
                Role.Admin,
                Role.Staff
            };

            var token = _jwt.CreateToken(
                o =>
                {
                    o.User.Permissions.AddRange(userPermissions);
                    o.User.Roles.AddRange(userRoles);
                    o.User.Claims.AddRange(userClaims);
                });

            await Send.OkAsync(
                new LoginResponse
                {
                    JWTToken = token,
                    ExpiryDate = expiryDate,
                    Permissions = Allow.NamesFor(userPermissions)
                });
            return;
        }
        AddError("Authentication Failed!");

        await Send.ErrorsAsync();
    }
}

public class Endpoint_V2 : Endpoint<EmptyRequest, object>
{
    public override void Configure()
    {
        Get(AppRoutes.admin_login);
        Version(2);
        AllowAnonymous();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Admin}"));
        Summary(s =>
        {
            s.Summary = "Admin login (v2)";
            s.Description = "Returns2 as a demo response for v2.";
        });
    }

    public override async Task HandleAsync(EmptyRequest r, CancellationToken ct)
        => await Send.OkAsync(2);
}