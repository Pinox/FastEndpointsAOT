using Shared.Contracts.Pipeline;

namespace Pipeline.PrePost;

public class LogPre : IPreProcessor<PipelineRequest>
{
    public Task PreProcessAsync(IPreProcessorContext<PipelineRequest> ctx, CancellationToken ct)
    {
        var name = ctx.Request?.Name ?? string.Empty;
        var logger = ctx.HttpContext.Resolve<ILogger<LogPre>>();
        logger.LogInformation("pre: {name}", name);
        return Task.CompletedTask;
    }
}

public class LogPost : IPostProcessor<PipelineRequest, object?>
{
    public Task PostProcessAsync(IPostProcessorContext<PipelineRequest, object?> ctx, CancellationToken ct)
    {
        var name = ctx.Request?.Name ?? string.Empty;
        var logger = ctx.HttpContext.Resolve<ILogger<LogPost>>();
        logger.LogInformation("post: {name}", name);
        return Task.CompletedTask;
    }
}

public class Endpoint : Endpoint<PipelineRequest>
{
    public override void Configure()
    {
        Post("/samples/pipeline/log");
        AllowAnonymous();
        PreProcessor<LogPre>();
        PostProcessor<LogPost>();
        Description(b => b.WithTags($"Heading:{ApiHeadings.Pipeline}"));
    }

    public override async Task HandleAsync(PipelineRequest r, CancellationToken ct)
        => await Send.OkAsync(new { r.Name, Message = "Logged" });
}
