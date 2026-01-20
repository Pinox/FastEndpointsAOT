#pragma warning disable RCS1074
namespace FastEndpoints;

/// <summary>
/// a request dto that doesn't have any properties.
/// Note: This is a class (not struct) for Native AOT compatibility.
/// Value types don't work with generic services in Native AOT.
/// </summary>
public sealed class EmptyRequest
{
    /// <summary>
    /// Dummy property required by FastEndpoints request binder (DTOs must have at least one property)
    /// </summary>
    public byte _dummy { get; set; }
}