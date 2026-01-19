using System.Diagnostics.CodeAnalysis;

namespace Web.Infrastructure;

/// <summary>
/// This class uses DynamicDependency attributes to tell the AOT trimmer to preserve
/// the public methods of all endpoint types. This is necessary because FastEndpoints
/// uses reflection (GetMethods) to check if endpoints implement Configure() at runtime.
/// 
/// The FastEndpoints.Generator source generator preserves TYPE references via typeof(),
/// but this doesn't preserve METHOD metadata. Without these attributes, the AOT trimmer
/// strips method metadata and FastEndpoints fails to detect Configure() implementations.
/// 
/// IMPORTANT: When adding new endpoints, you MUST add a corresponding DynamicDependency
/// attribute here, or the endpoint will fail at runtime in Release/AOT builds.
/// 
/// Alternative: Uncomment the line in rd.xml:
/// &lt;Assembly Name="Web" Dynamic="Required All" /&gt;
/// This preserves everything but increases binary size.
/// </summary>
public static class AotEndpointPreserver
{
    // ============ MIDDLEWARE ============
    // Middleware types need their public methods (Invoke/InvokeAsync) preserved
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.Auth.JwtBlacklistChecker))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.Hostings.AotResponseBufferingMiddleware))]
    
    // ============ PRE/POST PROCESSORS ============
    // Generic processors need to be preserved with their concrete type arguments
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Web.PipelineBehaviors.PreProcessors.SecurityProcessor<Shared.Contracts.Sales.Orders.RetrieveOrderRequest>))]
    
    // Pipeline processors
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Pipeline.PrePost.LogPre))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Pipeline.PrePost.LogPost))]
    
    // ============ ENDPOINTS ============
    // Endpoints need PublicMethods (for Configure/HandleAsync) AND PublicConstructors (for DI)
    // Admin endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Admin.Login.Endpoint_V1))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Admin.Login.Endpoint_V2))]
    
    // Basics endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Basics.Ping.PingEndpoint))]
    
    // Binding endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Binding.Multipart.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Binding.RouteAndQuery.Endpoint))]
    
    // Customers endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint.Endpoint_V1))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint.Endpoint_V2))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.UpdateWithHeader.Endpoint))]
    
    // Domain.Customers endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.Create.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.CreateWithPropertiesDI.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.List.Recent.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.List.Recent.Endpoint_V1))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.Update.Endpoint))]
    
    // Domain.Inventory endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.GetProduct.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.List.Recent.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.Manage.Create.Endpoint))]
    
    // Domain.Sales endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Sales.Orders.Create.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Sales.Orders.Retrieve.Endpoint))]
    
    // Inventory.Manage endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Inventory.Manage.Delete.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Inventory.Manage.Update.Endpoint))]
    
    // Pipeline endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Pipeline.PrePost.Endpoint))]
    
    // Security endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.ApiKey.FE_ApiKeyGet))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.ApiKey.FE_ApiKeyVerify))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Claims.VerifyClaim))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Cookie.FE_CookieGetCookie))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Cookie.FE_CookieVerify))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Diagnostics.WhoAmI))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Jwt.FE_JWTGetToken))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Jwt.FE_JWTVerify))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Policies.AdminOnly.AdminPolicyVerify))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Policies.UserOnly.UserPolicyVerify))]
    
    // Uploads endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.Save.Endpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.SaveTyped.Endpoint))]
    
    // Versioning endpoints
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Versioning.Sample.EndpointV1))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Versioning.Sample.EndpointV2))]
    
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
