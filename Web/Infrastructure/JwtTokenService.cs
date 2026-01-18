using FastEndpoints.Security;

namespace Web.Infrastructure;

public interface IJwtTokenService
{
    string CreateToken(Action<JwtCreationOptions> configure);
    DateTime GetDefaultExpiryUtcFromNow();
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly AuthSettings _settings;

    public JwtTokenService(IConfiguration config, AuthSettings settings)
    {
        _config = config;
        _settings = settings;
    }

    public DateTime GetDefaultExpiryUtcFromNow() => DateTime.UtcNow.AddMinutes(_settings.Jwt.ExpiresMinutes);

    public string CreateToken(Action<JwtCreationOptions> configure)
    {
        return JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _config["TokenKey"]!;
            o.ExpireAt = GetDefaultExpiryUtcFromNow();
            configure(o);
        });
    }
}
