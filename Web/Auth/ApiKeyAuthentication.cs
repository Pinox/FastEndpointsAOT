using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Web.Auth;

public static class ApiKeyAuth
{
 public const string Scheme = "ApiKey";
 public const string HeaderName = "X-API-Key";
}

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
 public ApiKeyAuthenticationHandler(
 IOptionsMonitor<AuthenticationSchemeOptions> options,
 ILoggerFactory logger,
 System.Text.Encodings.Web.UrlEncoder encoder)
 : base(options, logger, encoder)
 { }

 protected override Task<AuthenticateResult> HandleAuthenticateAsync()
 {
 if (!Request.Headers.TryGetValue(ApiKeyAuth.HeaderName, out var provided) || string.IsNullOrWhiteSpace(provided))
 {
 return Task.FromResult(AuthenticateResult.NoResult());
 }

 var cfg = Context.RequestServices.GetRequiredService<IConfiguration>();
 var expected = cfg["ApiKey"] ?? "sample-api-key";

 if (!string.Equals(provided.ToString(), expected, StringComparison.Ordinal))
 {
 return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
 }

 var claims = new System.Security.Claims.Claim[]
 {
 new System.Security.Claims.Claim(ClaimTypes.Name, "api-key-user"),
 };
 var identity = new ClaimsIdentity(claims, ApiKeyAuth.Scheme);
 var principal = new ClaimsPrincipal(identity);
 var ticket = new AuthenticationTicket(principal, ApiKeyAuth.Scheme);
 return Task.FromResult(AuthenticateResult.Success(ticket));
 }

 protected override Task HandleChallengeAsync(AuthenticationProperties properties)
 {
 Response.StatusCode = StatusCodes.Status401Unauthorized;
 Response.Headers["WWW-Authenticate"] = ApiKeyAuth.Scheme;
 return Task.CompletedTask;
 }
}
