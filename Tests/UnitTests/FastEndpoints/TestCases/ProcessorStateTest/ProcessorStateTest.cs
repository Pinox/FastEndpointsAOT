using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TestCases.ProcessorStateTest;

public class Thingy
{
    readonly Stopwatch _stopWatch;

    public int Id { get; set; }
    public string? Name { get; set; }
    public long Duration => _stopWatch.ElapsedMilliseconds;
    public bool GlobalStateApplied { get; set; }

    public Thingy()
    {
        _stopWatch = new();
        _stopWatch.Start();
    }
}

public class Request
{
    public int Id { get; set; }
}

public class FirstPreProcessor : PreProcessor<Request, Thingy>
{
    public override Task PreProcessAsync(IPreProcessorContext<Request> context, Thingy state, CancellationToken ct)
    {
        state.Id = context.Request!.Id;
        state.Name = "john doe";

        return Task.CompletedTask;
    }
}

public class SecondProcessor : FirstPreProcessor
{
    public override Task PreProcessAsync(IPreProcessorContext<Request> context, Thingy state, CancellationToken ct)
    {
        state.Name = "jane doe";
        return Task.CompletedTask;
    }
}

public class RequestDurationLogger : PostProcessor<Request, Thingy, string>
{
    public override Task PostProcessAsync(IPostProcessorContext<Request, string> context, Thingy state, CancellationToken ct)
    {
        var logger = context.HttpContext.Resolve<Microsoft.Extensions.Logging.ILogger<RequestDurationLogger>>();
        logger.LogInformation("Requst took: {@duration} ms.", state.Duration);

        return Task.CompletedTask;
    }
}

public class Endpoint : Endpoint<Request, string>
{
    public override void Configure()
    {
        Get("testcases/processor-state-sharing");
        AllowAnonymous();
        PreProcessors(
            new FirstPreProcessor(),
            new SecondProcessor());
        PostProcessors(new RequestDurationLogger());
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        var state = ProcessorState<Thingy>();
        await Task.Delay(100);
        await Send.OkAsync(state.Id + " " + state.Name + " " + state.GlobalStateApplied);
    }
}
