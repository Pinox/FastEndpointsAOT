using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Security;

/// <summary>
/// Request to verify a claim.
/// </summary>
public sealed class VerifyClaimRequest : IApiRequest<VerifyClaimRequest, VerifyClaimResponse>
{
    public string Type { get; set; } = "";
    public string? Value { get; set; }
    
    public static string Route => ApiRoutes.Security_Claims_Verify;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<VerifyClaimRequest> RequestTypeInfo => SharedJsonContext.Default.VerifyClaimRequest;
    public static JsonTypeInfo<VerifyClaimResponse> ResponseTypeInfo => SharedJsonContext.Default.VerifyClaimResponse;
}

/// <summary>
/// Response from verifying a claim.
/// </summary>
public sealed class VerifyClaimResponse
{
    public bool IsDevelopment { get; set; }
    public bool IsAuthenticated { get; set; }
    public string Type { get; set; } = "";
    public string? Value { get; set; }
    public bool HasType { get; set; }
    public bool HasTypeAndValue { get; set; }
    public int MatchedCount { get; set; }
}

/// <summary>
/// Request to get current user info.
/// </summary>
public sealed class WhoAmIRequest : IApiRequest<WhoAmIRequest, WhoAmIResponse>
{
    public static string Route => ApiRoutes.Security_WhoAmI;
    public static HttpMethod Method => HttpMethod.Get;
    public static JsonTypeInfo<WhoAmIRequest> RequestTypeInfo => SharedJsonContext.Default.WhoAmIRequest;
    public static JsonTypeInfo<WhoAmIResponse> ResponseTypeInfo => SharedJsonContext.Default.WhoAmIResponse;
}

/// <summary>
/// Response from the WhoAmI endpoint.
/// </summary>
public sealed class WhoAmIResponse
{
    public bool IsAuthenticated { get; set; }
    public string? AuthenticationType { get; set; }
    public string? Name { get; set; }
    public string? NameClaimType { get; set; }
    public string? RoleClaimType { get; set; }
    public bool IsAdminRole { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string? Email { get; set; }
    public ClaimKV[] Claims { get; set; } = Array.Empty<ClaimKV>();
    public string[] AcceptedAuthSchemes { get; set; } = Array.Empty<string>();

    // Cookie expiration times
    public string? CookieExpiresUtcFormatted { get; set; }
    public string? CookieExpiresUkFormatted { get; set; }

    // JWT expiration times
    public string? JwtExpiresUtcFormatted { get; set; }
    public string? JwtExpiresUkFormatted { get; set; }
}

/// <summary>
/// Represents a claim type-value pair.
/// </summary>
public readonly record struct ClaimKV(string Type, string Value);
