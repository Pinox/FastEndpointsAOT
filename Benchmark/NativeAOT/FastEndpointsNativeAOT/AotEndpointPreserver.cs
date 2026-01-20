using System.Diagnostics.CodeAnalysis;

namespace FastEndpointsNativeAOT;

/// <summary>
/// This class uses DynamicDependency attributes to tell the AOT trimmer to preserve
/// the public methods of all endpoint types. This is necessary because FastEndpoints
/// uses reflection (GetMethods) to check if endpoints implement Configure() at runtime.
/// </summary>
public static class AotEndpointPreserver
{
    // Benchmark endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(ReadyEndpoint))]

    /// <summary>
    /// This method must be called during startup to ensure the DynamicDependency attributes
    /// are recognized by the AOT trimmer. The method itself does nothing at runtime.
    /// </summary>
    public static void EnsureEndpointsPreserved()
    {
        // This method body is intentionally empty.
        // The DynamicDependency attributes above tell the AOT trimmer to preserve
        // the public methods of the specified types. Simply having these attributes
        // on this class is enough - but calling this method ensures the class itself
        // isn't trimmed away.
    }
}