using System.Text.Json.Serialization.Metadata;

namespace Shared.Contracts.Admin;

/// <summary>
/// Base request for admin login endpoint.
/// Web endpoint may add additional properties like [JsonIgnore] getter-only props.
/// </summary>
public class LoginRequestBase
{
    /// <summary>
    /// The admin username/email
    /// </summary>
    /// <example>admin@example.com</example>
    public virtual string email { get; set; } = "";

    /// <summary>
    /// The admin password
    /// </summary>
    /// <example>password123</example>
    public virtual string Password { get; set; } = "";
}

/// <summary>
/// Client-facing login request that implements IApiRequest for type-safe HTTP calls.
/// Use this with HttpClient.SendAsJsonAsync() for AOT-compatible API calls.
/// </summary>
/// <example>
/// var (response, result) = await client.SendAsJsonAsync(new LoginRequest
/// {
///     email = "admin@example.com",
///     Password = "password123"
/// });
/// </example>
public sealed class LoginRequest : LoginRequestBase, IApiRequest<LoginRequest, LoginResponse>
{
    public static string Route => ApiRoutes.Admin_Login;
    public static HttpMethod Method => HttpMethod.Post;
    public static JsonTypeInfo<LoginRequest> RequestTypeInfo => SharedJsonContext.Default.LoginRequest;
    public static JsonTypeInfo<LoginResponse> ResponseTypeInfo => SharedJsonContext.Default.LoginResponse;
}

/// <summary>
/// Response from admin login endpoint.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// The JWT token
    /// </summary>
    public string? JWTToken { get; set; }

    /// <summary>
    /// Token expiry date
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// User permissions
    /// </summary>
    public IEnumerable<string> Permissions { get; set; } = [];
}
