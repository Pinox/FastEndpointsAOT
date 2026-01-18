namespace Shared.Contracts.Pipeline;

/// <summary>
/// Request for pipeline pre/post processor sample.
/// </summary>
public sealed class PipelineRequest
{
    public string Name { get; set; } = "";
}
