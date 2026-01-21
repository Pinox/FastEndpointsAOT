using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Web.Infrastructure;

/// <summary>
/// Preserves ASP.NET Core middleware method metadata for AOT compilation.
/// 
/// Middleware types are not discovered by FastEndpoints.Generator (they don't implement
/// FastEndpoints interfaces), so they need manual preservation.
/// 
/// The module initializer ensures DynamicDependency attributes are processed at assembly load.
/// </summary>
file static class AotMiddlewareModuleInitializer
{
    [ModuleInitializer]
    internal static void Initialize() => AotMiddlewarePreserver.EnsureMiddlewarePreserved();
}

/// <summary>
/// Middleware types that need their public methods preserved for AOT.
/// Add any custom middleware here that uses reflection-based invocation.
/// </summary>
public static class AotMiddlewarePreserver
{
    // Middleware
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.Auth.JwtBlacklistChecker))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.Hostings.AotResponseBufferingMiddleware))]
    
    // Generic pre/post processors with concrete type arguments (not discovered by generator)
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.PipelineBehaviors.PreProcessors.SecurityProcessor<Shared.Contracts.Sales.Orders.RetrieveOrderRequest>))]
    public static void EnsureMiddlewarePreserved() { }
}
