namespace Web.Infrastructure;

public sealed class AuthSettings
{
    public JwtSettings Jwt { get; init; } = new();
    public CookieSettings Cookies { get; init; } = new();

    public sealed class JwtSettings
    {
        public int ExpiresMinutes { get; init; } = 60;
    }

    public sealed class CookieSettings
    {
        public int ExpiresMinutes { get; init; } = 60;
        public bool SlidingExpiration { get; init; } = true;
        public string Name { get; init; } = "auth";
        public bool IsPersistent { get; init; } = true;
    }
}
