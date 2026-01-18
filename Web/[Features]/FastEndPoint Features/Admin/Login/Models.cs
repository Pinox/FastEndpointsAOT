using FluentValidation;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Shared.Contracts.Admin;

namespace Admin.Login;

// Serialization is handled via Web.Serialization.AppJsonContext

/// <summary>
/// Derived request adding Web-specific properties.
/// </summary>
public class Request : LoginRequestBase
{
    /// <summary>
    /// Server-only: getter-only property for testing
    /// </summary>
    [JsonIgnore]
    public string GetterOnlyProp => "test";
}

public class Validator : Validator<Request>
{
    public Validator(IConfiguration config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        RuleFor(x => x.email)
            .Must(_ => config["TokenKey"] == "some long secret key to sign jwt tokens with")
            .WithMessage("config didn't resolve correctly!")
            .MaximumLength(50)
            .WithMessage("cannot exceed 50 chars!");

        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Username is required!")
            .MinimumLength(3).WithMessage("Username too short!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(3).WithMessage("Password too short!");

        var logger = Resolve<ILogger<Validator>>();
        logger.LogError("resolving from validator works!");
    }
}

// Response type moved to Shared.Contracts.Admin.LoginResponse