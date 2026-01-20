using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using FastEndpoints.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace FastEndpoints;

//key: tCommand
//val: command handler definition

/// <summary>
/// registry for command handlers. maps command types to their handler definitions.
/// </summary>
public sealed class CommandHandlerRegistry : ConcurrentDictionary<Type, CommandHandlerDefinition>;

/// <summary>
/// extension methods for command execution
/// </summary>
public static class CommandExtensions
{
#pragma warning disable CS0649 // Field is never assigned to
    internal static bool TestHandlersPresent;
#pragma warning restore CS0649
    internal static bool UsePreGeneratedExecutors;

    /// <summary>
    /// executes the command that does not return a result
    /// </summary>
    /// <param name="command">the command to execute</param>
    /// <param name="ct">optional cancellation token</param>
    /// <exception cref="InvalidOperationException">thrown when a handler for the command cannot be instantiated</exception>
    public static Task ExecuteAsync<TCommand>(this TCommand command, CancellationToken ct = default) where TCommand : class, ICommand
        => ExecuteAsync<Void>(command, ct);

    /// <summary>
    /// executes the command and returns a result
    /// </summary>
    /// <typeparam name="TResult">the type of the returned result</typeparam>
    /// <param name="command">the command to execute</param>
    /// <param name="ct">optional cancellation token</param>
    /// <exception cref="InvalidOperationException">thrown when a handler for the command cannot be instantiated</exception>
    public static Task<TResult> ExecuteAsync<TResult>(this ICommand<TResult> command, CancellationToken ct = default)
    {
        var tCommand = command.GetType();
        var registry = ServiceResolver.Instance.Resolve<CommandHandlerRegistry>();

        registry.TryGetValue(tCommand, out var def);

        InitGenericHandler<TResult>(ref def, tCommand, registry);

        if (def is null)
            throw new InvalidOperationException($"Unable to create an instance of the handler for command [{tCommand.FullName}]");

        // Use pre-generated executor if available (AOT-safe path)
        if (def.HandlerExecutor is null)
        {
            if (UsePreGeneratedExecutors)
            {
                throw new InvalidOperationException(
                    $"No pre-generated executor found for command [{tCommand.FullName}]. " +
                    "When using AOT, ensure RegisterCommandExecutors() is called with source-generated executors.");
            }

            def.HandlerExecutor = CreateExecutorFallback(tCommand, typeof(TResult));
        }

        // ReSharper disable once InvertIf
        if (TestHandlersPresent)
        {
            var tHandlerInterface = CreateHandlerInterfaceType(tCommand, typeof(TResult));
            def.HandlerType = ServiceResolver.Instance.TryResolve(tHandlerInterface)?.GetType() ?? def.HandlerType;
        }

        return ((ICommandHandlerExecutor<TResult>)def.HandlerExecutor).Execute(command, def.HandlerType, ct);
    }

    [RequiresDynamicCode("MakeGenericType is not AOT compatible. Use source-generated executors instead.")]
    [RequiresUnreferencedCode("MakeGenericType may break in trimmed applications.")]
    static object CreateExecutorFallback(Type tCommand, Type tResult)
        => ServiceResolver.Instance.CreateSingleton(Types.CommandHandlerExecutorOf2.MakeGenericType(tCommand, tResult));

    [RequiresDynamicCode("MakeGenericType is not AOT compatible.")]
    [RequiresUnreferencedCode("MakeGenericType may break in trimmed applications.")]
    static Type CreateHandlerInterfaceType(Type tCommand, Type tResult)
        => Types.ICommandHandlerOf2.MakeGenericType(tCommand, tResult);

    /// <summary>
    /// registers a fake command handler for unit testing purposes
    /// </summary>
    /// <typeparam name="TCommand">type of the command</typeparam>
    /// <param name="handler">a fake handler instance</param>
    public static void RegisterForTesting<TCommand>(this ICommandHandler<TCommand, Void> handler) where TCommand : ICommand
    {
        var tCommand = typeof(TCommand);
        var registry = ServiceResolver.Instance.Resolve<CommandHandlerRegistry>();

        registry[tCommand] = new(handler.GetType())
        {
            HandlerExecutor = new CommandHandlerExecutor<TCommand, Void>(
                ServiceResolver.Instance.Resolve<IEnumerable<ICommandMiddleware<TCommand, Void>>>(),
                handler)
        };
    }

    /// <summary>
    /// registers a fake command handler for unit testing purposes
    /// </summary>
    /// <typeparam name="TCommand">type of the command</typeparam>
    /// <typeparam name="TResult">type of the result being returned by the handler</typeparam>
    /// <param name="handler">a fake handler instance</param>
    public static void RegisterForTesting<TCommand, TResult>(this ICommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
    {
        var tCommand = typeof(TCommand);
        var registry = ServiceResolver.Instance.Resolve<CommandHandlerRegistry>();

        registry[tCommand] = new(handler.GetType())
        {
            HandlerExecutor = new CommandHandlerExecutor<TCommand, TResult>(
                ServiceResolver.Instance.Resolve<IEnumerable<ICommandMiddleware<TCommand, TResult>>>(),
                handler)
        };
    }

    /// <param name="sp">the service provider</param>
    extension(IServiceProvider sp)
    {
        /// <summary>
        /// register a generic command handler for a generic command
        /// </summary>
        /// <typeparam name="TCommand">the type of the command</typeparam>
        /// <typeparam name="THandler">the type of the command handler</typeparam>
        /// <returns>the service provider for chaining</returns>
        [SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known")]
        public IServiceProvider RegisterGenericCommand<TCommand, THandler>() where TCommand : ICommand where THandler : ICommandHandler
            => RegisterGenericCommand(sp, typeof(TCommand), typeof(THandler));

        /// <summary>
        /// register a generic command handler for a generic command
        /// </summary>
        /// <param name="genericCommandType">
        /// the open generic type of the command. ex: <c> typeof(MyCommand&lt;&gt;) </c>
        /// </param>
        /// <param name="genericHandlerType">the open generic type of the command handler. ex: <c> typeof(MyCommandHandler&lt;,&gt;) </c></param>
        /// <returns>the service provider for chaining</returns>
        public IServiceProvider RegisterGenericCommand(Type genericCommandType, Type genericHandlerType)
        {
            var registry = sp.GetRequiredService<CommandHandlerRegistry>();

            registry[genericCommandType] = new(genericHandlerType);

            return sp;
        }
    }

    /// <summary>
    /// register a common middleware pipeline for command handlers. the middleware can be created as open generic classes that implement the
    /// <see cref="ICommandMiddleware{TCommand,TResult}" /> interface as well as closed generic classes implementing the same interface.
    /// </summary>
    /// <param name="services">the service collection</param>
    /// <param name="config">configuration action for adding middleware components to the pipeline</param>
    /// <returns>the service collection for chaining</returns>
    public static IServiceCollection AddCommandMiddleware(this IServiceCollection services, Action<CommandMiddlewareConfig> config)
    {
        var c = new CommandMiddlewareConfig();
        config(c);

        if (c.Middleware.Count == 0)
            throw new ArgumentNullException(nameof(config), "Please add some command middleware to the pipeline!");

        foreach (var mw in c.Middleware)
            services.AddTransient(mw.tInterface, mw.tImplementation);

        return services;
    }

    /// <summary>
    /// enables AOT mode for command execution, requiring all executors to be pre-generated.
    /// when enabled, MakeGenericType will not be used and an exception will be thrown if a pre-generated executor is not found.
    /// </summary>
    public static void EnableAotMode()
    {
        UsePreGeneratedExecutors = true;
    }

    /// <summary>
    /// Registers pre-generated command handler executors with the command registry.
    /// This method should be called at application startup with a delegate that calls the source-generated RegisterCommandExecutors method.
    /// </summary>
    /// <param name="sp">The service provider to resolve middleware dependencies.</param>
    /// <param name="registrationAction">
    /// A delegate that receives the registry and service provider. 
    /// Typically this calls the source-generated GeneratedReflection.RegisterCommandExecutors method.
    /// </param>
    /// <example>
    /// <code>
    /// // In Program.cs after app.UseFastEndpoints():
    /// app.RegisterCommandExecutors((registry, sp) => GeneratedReflection.RegisterCommandExecutors(registry, sp));
    /// CommandExtensions.EnableAotMode(); // Optional: require all commands to use pre-generated executors
    /// </code>
    /// </example>
    public static IServiceProvider RegisterCommandExecutors(
        this IServiceProvider sp,
        Action<CommandHandlerRegistry, IServiceProvider> registrationAction)
    {
        var registry = sp.GetRequiredService<CommandHandlerRegistry>();
        registrationAction(registry, sp);
        return sp;
    }

    [RequiresDynamicCode("MakeGenericType is not AOT compatible. Use source-generated executors for open generic commands.")]
    [RequiresUnreferencedCode("MakeGenericType may break in trimmed applications.")]
    static void InitGenericHandler<TResult>(ref CommandHandlerDefinition? def, Type tCommand, CommandHandlerRegistry registry)
    {
        if (def is not null || !tCommand.IsGenericType)
            return;

        // Check if a pre-generated closed generic handler is already registered
        if (registry.TryGetValue(tCommand, out def))
            return;

        if (UsePreGeneratedExecutors)
        {
            throw new InvalidOperationException(
                $"No pre-generated handler found for generic command [{tCommand.FullName}]. " +
                "When using AOT, ensure all generic command usages are discovered by the source generator. " +
                "Alternatively, manually register closed generic handlers at startup.");
        }

        var tGenCmd = tCommand.GetGenericTypeDefinition();

        if (!registry.TryGetValue(tGenCmd, out var genDef))
            throw new InvalidOperationException($"No generic handler registered for generic command type: [{tGenCmd.FullName}]");

        var tHnd = genDef.HandlerType.MakeGenericType(tCommand.GetGenericArguments());
        var tRes = typeof(TResult);
        var tTargetIfc = tRes == Types.VoidResult
                             ? Types.ICommandHandlerOf1.MakeGenericType(tCommand)
                             : Types.ICommandHandlerOf2.MakeGenericType(tCommand, tRes);

        if (!tHnd.IsAssignableTo(tTargetIfc))
            throw new InvalidOperationException($"The registered generic handler for the generic command [{tGenCmd.FullName}] is not the correct type!");

        def = registry[tCommand] = new(tHnd);
    }
}