using System.Security.Claims;

namespace Security.Claims;

// GET /security/claims/verify?type=...&value=... -> verifies current user has a specific claim (optionally with a specific value)
public sealed class VerifyClaim : Endpoint<VerifyClaimRequest, VerifyClaimResponse>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public VerifyClaim(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.security_claims_verify);
        // accept both cookie and jwt for verification
        AuthSchemes("Cookies", "Bearer");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(VerifyClaim);
            s.Description = "Verifies that the current user has a specific claim type (and optionally a specific value). Only available in Development.";
            s.ExampleRequest = new VerifyClaimRequest { Type = ClaimTypes.Email, Value = "someone@example.com" };
        });
    }

    public override Task HandleAsync(VerifyClaimRequest req, CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
            return Send.NotFoundAsync();

        if (string.IsNullOrWhiteSpace(req.Type))
        {
            AddError(r => r.Type, "'type' query parameter is required");
            return Send.ErrorsAsync();
        }

        var principal = User;
        var identity = principal?.Identity as ClaimsIdentity;
        var isAuthenticated = identity?.IsAuthenticated ?? false;

        bool hasType = false;
        bool hasTypeAndValue = false;
        int matchedCount = 0;

        if (isAuthenticated)
        {
            var claims = principal!.Claims.Where(c => string.Equals(c.Type, req.Type, StringComparison.OrdinalIgnoreCase));
            hasType = claims.Any();
            if (!string.IsNullOrWhiteSpace(req.Value))
            {
                claims = claims.Where(c => string.Equals(c.Value, req.Value, StringComparison.OrdinalIgnoreCase));
                hasTypeAndValue = claims.Any();
            }
            matchedCount = claims.Count();
        }

        Response = new VerifyClaimResponse
        {
            IsDevelopment = _appEnv.IsDevelopment,
            IsAuthenticated = isAuthenticated,
            Type = req.Type,
            Value = req.Value,
            HasType = hasType,
            HasTypeAndValue = hasTypeAndValue,
            MatchedCount = matchedCount
        };

        return Send.OkAsync(Response);
    }
}

public sealed class VerifyClaimRequest
{
    // claim type to look for (e.g., ClaimTypes.Email or "email")
    public string Type { get; set; } = string.Empty;
    // optional value to match
    public string? Value { get; set; }
}

public sealed class VerifyClaimResponse
{
    public bool IsDevelopment { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Type { get; set; }
    public string? Value { get; set; }
    public bool HasType { get; set; }
    public bool HasTypeAndValue { get; set; }
    public int MatchedCount { get; set; }
}
