namespace FastEndpoints;

//NOTE: CommandHandlerExecutor<> class is singleton
//      (cached in CommandHandlerDefinition.HandlerExecutor property)

/// <summary>
/// interface for command handler executors that execute commands and return results.
/// </summary>
/// <typeparam name="TResult">the result type of the command</typeparam>
public interface ICommandHandlerExecutor<TResult>
{
    /// <summary>
    /// executes a command and returns the result.
    /// </summary>
    Task<TResult> Execute(ICommand<TResult> command, Type handlerType, CancellationToken ct);
}

/// <summary>
/// executes a command handler with optional middleware pipeline.
/// this class is singleton and cached in CommandHandlerDefinition.HandlerExecutor.
/// </summary>
/// <typeparam name="TCommand">the command type</typeparam>
/// <typeparam name="TResult">the result type</typeparam>
public sealed class CommandHandlerExecutor<TCommand, TResult>(IEnumerable<ICommandMiddleware<TCommand, TResult>>? m = null, ICommandHandler<TCommand, TResult>? handler = null)
    : ICommandHandlerExecutor<TResult> where TCommand : ICommand<TResult>
{
    readonly Type[] _tMiddlewares = m?.Select(x => x.GetType()).ToArray() ?? [];

    /// <inheritdoc />
    public Task<TResult> Execute(ICommand<TResult> command, Type tCommandHandler, CancellationToken ct)
    {
        var cmdHandler = handler ?? //handler is not null for unit tests
                         (ICommandHandler<TCommand, TResult>)Cfg.ServiceResolver.CreateInstance(tCommandHandler);

        return InvokeMiddleware(0);

        Task<TResult> InvokeMiddleware(int index)
        {
            return index == _tMiddlewares.Length
                       ? cmdHandler.ExecuteAsync((TCommand)command, ct)
                       : ((ICommandMiddleware<TCommand, TResult>)Cfg.ServiceResolver.CreateInstance(_tMiddlewares[index])).ExecuteAsync(
                           (TCommand)command,
                           () => InvokeMiddleware(index + 1),
                           ct);
        }
    }
}