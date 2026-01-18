using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;
using Shared.Contracts.Security;

namespace Security.Diagnostics;

// GET /security/whoami -> dumps current user's claims/roles for troubleshooting auth
public sealed class WhoAmI : EndpointWithoutRequest<WhoAmIResponse>
{
    private readonly Web.Infrastructure.AppEnv _appEnv;
    public WhoAmI(Web.Infrastructure.AppEnv appEnv) => _appEnv = appEnv;

    public override void Configure()
    {
        Get(AppRoutes.security_whoami);
        // accept both cookie and jwt for diagnostics
        AuthSchemes("Cookies", "Bearer");
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(WhoAmI);
            s.Description = "Echoes the current user's authentication/claims/roles for troubleshooting. Only available in Development.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_appEnv.IsDevelopment)
        {
            await Send.NotFoundAsync();
            return;
        }

        var principal = User;
        var identity = principal?.Identity as ClaimsIdentity;
        var isAuthenticated = identity?.IsAuthenticated ?? false;

        var roleClaimType = identity?.RoleClaimType ?? ClaimsIdentity.DefaultRoleClaimType; // falls back to ClaimTypes.Role
        var roleTypesToCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            roleClaimType,
            ClaimTypes.Role,
            "role"
        };

        var claims = principal?.Claims?.Select(c => new ClaimKV(c.Type, c.Value)).ToArray() ?? Array.Empty<ClaimKV>();
        var roles = principal?.Claims?.Where(c => roleTypesToCheck.Contains(c.Type))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? Array.Empty<string>();

        var email = principal?.Claims?.FirstOrDefault(c => string.Equals(c.Type, Web.Auth.Claim.Email, StringComparison.OrdinalIgnoreCase))?.Value
                    ?? principal?.Claims?.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;

        // determine cookie/jwt expiration timestamps if available (not returned directly; only formatted)
        DateTimeOffset? cookieExpiresUtc = null;
        try
        {
            var cookieAuth = await HttpContext.AuthenticateAsync("Cookies");
            cookieExpiresUtc = cookieAuth?.Properties?.ExpiresUtc;
        }
        catch { }

        DateTimeOffset? jwtExpiresUtc = null;
        var expClaim = principal?.Claims?.FirstOrDefault(c => string.Equals(c.Type, "exp", StringComparison.OrdinalIgnoreCase));
        if (expClaim is not null && long.TryParse(expClaim.Value, out var expSeconds))
        {
            try { jwtExpiresUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds); } catch { }
        }
        if (jwtExpiresUtc is null)
        {
            try
            {
                var bearerAuth = await HttpContext.AuthenticateAsync("Bearer");
                jwtExpiresUtc = bearerAuth?.Properties?.ExpiresUtc;
            }
            catch { }
        }

        // formatting: dd MMM yyyy hh:mm with uppercase AM/PM
        const string fmt = "dd MMM yyyy hh:mm tt";
        var enGbUpper = (CultureInfo)CultureInfo.GetCultureInfo("en-GB").Clone();
        enGbUpper.DateTimeFormat.AMDesignator = "AM";
        enGbUpper.DateTimeFormat.PMDesignator = "PM";

        string? cookieExpiresUtcFormatted = cookieExpiresUtc.HasValue
            ? cookieExpiresUtc.Value.ToUniversalTime().ToString(fmt, enGbUpper)
            : null;
        string? cookieExpiresUkFormatted = null;

        string? jwtExpiresUtcFormatted = jwtExpiresUtc.HasValue
            ? jwtExpiresUtc.Value.ToUniversalTime().ToString(fmt, enGbUpper)
            : null;
        string? jwtExpiresUkFormatted = null;

        // also provide UK (Europe/London) time
        TimeZoneInfo? ukTz = null;
        try { ukTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/London"); } catch { }
        if (ukTz is null)
        {
            try { ukTz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"); } catch { }
        }

        if (cookieExpiresUtc.HasValue && ukTz is not null)
        {
            var ukTime = TimeZoneInfo.ConvertTime(cookieExpiresUtc.Value.UtcDateTime, ukTz);
            cookieExpiresUkFormatted = ukTime.ToString(fmt, enGbUpper);
        }

        if (jwtExpiresUtc.HasValue && ukTz is not null)
        {
            var ukTime = TimeZoneInfo.ConvertTime(jwtExpiresUtc.Value.UtcDateTime, ukTz);
            jwtExpiresUkFormatted = ukTime.ToString(fmt, enGbUpper);
        }

        Response = new WhoAmIResponse
        {
            IsAuthenticated = isAuthenticated,
            AuthenticationType = identity?.AuthenticationType,
            Name = principal?.Identity?.Name,
            NameClaimType = identity?.NameClaimType,
            RoleClaimType = roleClaimType,
            IsAdminRole = roles.Contains(Web.Auth.Role.Admin, StringComparer.OrdinalIgnoreCase) || principal?.IsInRole(Web.Auth.Role.Admin) == true,
            Roles = roles,
            Email = email,
            Claims = claims,
            AcceptedAuthSchemes = new[] { "Cookies", "Bearer" },

            // cookie times first
            CookieExpiresUtcFormatted = cookieExpiresUtcFormatted,
            CookieExpiresUkFormatted = cookieExpiresUkFormatted,

            // then jwt times
            JwtExpiresUtcFormatted = jwtExpiresUtcFormatted,
            JwtExpiresUkFormatted = jwtExpiresUkFormatted
        };

        await Send.OkAsync(Response);
    }
}

// WhoAmIResponse and ClaimKV moved to Shared.Contracts.Security
