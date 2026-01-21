namespace FastEndpoints;

/// <summary>
/// Provides registration methods for source-generated event publishers.
/// This class exists separately from EventExtensions to avoid naming conflicts
/// between FastEndpoints.Messaging and FastEndpoints.Messaging.Remote assemblies.
/// </summary>
public static class EventPublisherRegistry
{
    // Delegate for resolving source-generated publishers (set by generated code)
    private static Func<Type, Func<IEvent, Mode, CancellationToken, Task>?>? _generatedPublisherResolver;

    /// <summary>
    /// Registers the source-generated event publisher resolver.
    /// This is called automatically by the generated EventPublishers class during app startup via module initializer.
    /// </summary>
    /// <param name="resolver">A function that returns a publish delegate for a given event type, or null if not found.</param>
    public static void RegisterGeneratedPublishers(Func<Type, Func<IEvent, Mode, CancellationToken, Task>?> resolver)
    {
        _generatedPublisherResolver = resolver;
    }

    /// <summary>
    /// Tries to get a pre-generated publish delegate for the specified event type.
    /// </summary>
    /// <param name="eventType">The type of the event.</param>
    /// <returns>The publish delegate if found, otherwise null.</returns>
    internal static Func<IEvent, Mode, CancellationToken, Task>? GetPublisher(Type eventType)
    {
        return _generatedPublisherResolver?.Invoke(eventType);
    }

    /// <summary>
    /// Publishes an event using the EventBus. This method is used by generated code
    /// to avoid calling EventExtensions directly (which has naming conflicts).
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event to publish.</param>
    /// <param name="waitMode">The wait mode for subscribers.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>A task representing the publish operation.</returns>
    public static Task PublishEvent<TEvent>(TEvent eventModel, Mode waitMode, CancellationToken cancellation) where TEvent : IEvent
        => ServiceResolver.Instance.Resolve<EventBus<TEvent>>().PublishAsync(eventModel, waitMode, cancellation);
}
