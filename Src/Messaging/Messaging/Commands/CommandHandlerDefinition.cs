namespace FastEndpoints;

/// <summary>
/// definition for a command handler, containing the handler type and executor.
/// </summary>
public sealed class CommandHandlerDefinition
{
    /// <summary>
    /// the type of the command handler.
    /// </summary>
    public Type HandlerType { get; set; }

    /// <summary>
    /// the cached executor instance for this command handler.
    /// </summary>
    public object? HandlerExecutor { get; set; }

    /// <summary>
    /// creates a new command handler definition for the specified handler type.
    /// </summary>
    /// <param name="handlerType">the type of the command handler</param>
    public CommandHandlerDefinition(Type handlerType)
    {
        HandlerType = handlerType;
    }
}