using Microsoft.Extensions.Hosting;

namespace Web.Infrastructure;

public sealed class AppEnv
{
    /// <summary>
    /// The logical environment name (for example: "Development", "Staging", or "Production").
    /// </summary>
    public string EnvironmentName { get; }

    /// <summary>
    /// True when <see cref="EnvironmentName"/> equals "Development" (case-insensitive).
    /// </summary>
    public bool IsDevelopment { get; }

    /// <summary>
    /// True when <see cref="EnvironmentName"/> equals "Production" (case-insensitive).
    /// </summary>
    public bool IsProduction { get; }

    /// <summary>
    /// True when <see cref="EnvironmentName"/> equals "Staging" (case-insensitive).
    /// </summary>
    public bool IsStaging { get; }

    /// <summary>
    /// Creates a new instance using the host-provided environment.
    /// </summary>
    /// <param name="env">The current host environment.</param>
    public AppEnv(IHostEnvironment env)
    {
        EnvironmentName = env.EnvironmentName;
        IsDevelopment = env.IsDevelopment();
        IsProduction = env.IsProduction();
        IsStaging = env.IsStaging();
    }
}