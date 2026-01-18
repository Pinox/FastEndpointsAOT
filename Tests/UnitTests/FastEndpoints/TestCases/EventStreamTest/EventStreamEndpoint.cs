using System.Runtime.CompilerServices;

namespace TestCases.EventStreamTest;

public sealed record SomeNotification(string Name);

public sealed record Request(string EventName, SomeNotification[] Notifications);

public sealed class EventStreamEndpoint : Endpoint<Request>
{
    public override void Configure()
    {
        Post("test-cases/event-stream");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        static async IAsyncEnumerable<SomeNotification> CreateEventStream(Request request, [EnumeratorCancellation] CancellationToken ct)
        {
            foreach (var notification in request.Notifications)
            {
                yield return notification;
                await Task.Delay(100, ct);
            }
        }

        await Send.EventStreamAsync(request.EventName, CreateEventStream(request, ct), ct);
    }
}
