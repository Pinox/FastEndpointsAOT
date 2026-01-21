namespace FastEndpoints;

/// <summary>
/// Registry for source-generated command executor factories.
/// Used by the source generator to register pre-compiled command executors
/// that are automatically invoked when <c>UseGeneratedCommandExecutors</c> is enabled.
/// </summary>
public static class CommandExecutorRegistry
{
    private static readonly List<Action<CommandHandlerRegistry, IServiceProvider>> Registrations = [];

    /// <summary>
    /// Registers a command executor factory that will be called when
    /// <c>Endpoints.UseGeneratedCommandExecutors</c> is enabled in UseFastEndpoints config.
    /// This is typically called by source-generated module initializers.
    /// </summary>
    /// <param name="registration">
    /// A delegate that registers command executors with the registry.
    /// </param>
    public static void Register(Action<CommandHandlerRegistry, IServiceProvider> registration)
        => Registrations.Add(registration);

    /// <summary>
    /// Gets whether any command executor registrations have been registered by source generators.
    /// </summary>
    public static bool HasRegistrations => Registrations.Count > 0;

    /// <summary>
    /// Executes all registered command executor factories.
    /// Called internally by FastEndpoints when <c>UseGeneratedCommandExecutors</c> is enabled.
    /// </summary>
    /// <param name="registry">The command handler registry to populate.</param>
    /// <param name="sp">The service provider for resolving middleware.</param>
    internal static void RegisterAll(CommandHandlerRegistry registry, IServiceProvider sp)
    {
        foreach (var registration in Registrations)
            registration(registry, sp);
    }
}
