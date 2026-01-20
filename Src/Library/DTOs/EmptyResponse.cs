#pragma warning disable RCS1074
namespace FastEndpoints;

/// <summary>
/// a response dto that doesn't have any properties.
/// Note: This is a class (not struct) for Native AOT compatibility.
/// Value types used as generic type arguments can cause issues in Native AOT.
/// </summary>
public sealed class EmptyResponse
{
    /// <summary>
    /// Dummy property for serialization compatibility
    /// </summary>
    public byte _dummy { get; set; }
}