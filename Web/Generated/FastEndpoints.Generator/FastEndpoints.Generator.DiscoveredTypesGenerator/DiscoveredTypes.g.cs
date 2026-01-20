#pragma warning disable CS0618

namespace Web;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Auto-generated list of discovered FastEndpoints types.
/// The DynamicDependency attributes ensure AOT/trimming preserves method and constructor metadata.
/// </summary>
public static class DiscoveredTypes
{    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Admin.Login.Endpoint_V1))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Admin.Login.Endpoint_V2))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Admin.Login.Validator))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Basics.Ping.PingEndpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Binding.Multipart.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Binding.RouteAndQuery.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.EventHandlers.SendOrderConfirmation))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint.Endpoint_V1))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.Login.Endpoint.Endpoint_V2))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Customers.UpdateWithHeader.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.Create.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.CreateWithPropertiesDI.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.List.Recent.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.List.Recent.Endpoint_V1))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Customers.Update.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.GetProduct.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.List.Recent.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.Manage.Create.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Inventory.Manage.Create.Validator))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Sales.Orders.Create.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Sales.Orders.Retrieve.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Domain.Shipping.EventHandlers.StartOrderProcessing))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Inventory.Manage.Delete.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Inventory.Manage.Update.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Inventory.Manage.Update.Validator))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Pipeline.PrePost.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.ApiKey.FE_ApiKeyGet))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.ApiKey.FE_ApiKeyVerify))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Claims.VerifyClaim))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Cookie.FE_CookieGetCookie))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Cookie.FE_CookieVerify))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Diagnostics.WhoAmI))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Jwt.FE_JWTGetToken))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Jwt.FE_JWTVerify))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Policies.AdminOnly.AdminPolicyVerify))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Security.Policies.UserOnly.UserPolicyVerify))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandBusTest.EchoCommandHandler))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandBusTest.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandBusTest.SomeCommandHandler))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandBusTest.VoidCommandHandler))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandHandlerTest.ConcreteCmdEndpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandHandlerTest.GetHandler))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandHandlerTest.GetHandlerWithoutResult))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TestCases.CommandHandlerTest.MakeFullName))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.Save.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.Save.Validator))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.SaveTyped.Endpoint))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Uploads.Image.SaveTyped.Validator))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Versioning.Sample.EndpointV1))]    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Versioning.Sample.EndpointV2))]    /// <summary>
    /// All discovered endpoint, validator, event handler, and command handler types.
    /// Each type has its public methods and constructors preserved for AOT compatibility.
    /// </summary>
    public static readonly List<Type> All =
    [
        typeof(Admin.Login.Endpoint_V1),
        typeof(Admin.Login.Endpoint_V2),
        typeof(Admin.Login.Validator),
        typeof(Basics.Ping.PingEndpoint),
        typeof(Binding.Multipart.Endpoint),
        typeof(Binding.RouteAndQuery.Endpoint),
        typeof(Customers.EventHandlers.SendOrderConfirmation),
        typeof(Customers.Login.Endpoint),
        typeof(Customers.Login.Endpoint.Endpoint_V1),
        typeof(Customers.Login.Endpoint.Endpoint_V2),
        typeof(Customers.UpdateWithHeader.Endpoint),
        typeof(Domain.Customers.Create.Endpoint),
        typeof(Domain.Customers.CreateWithPropertiesDI.Endpoint),
        typeof(Domain.Customers.List.Recent.Endpoint),
        typeof(Domain.Customers.List.Recent.Endpoint_V1),
        typeof(Domain.Customers.Update.Endpoint),
        typeof(Domain.Inventory.GetProduct.Endpoint),
        typeof(Domain.Inventory.List.Recent.Endpoint),
        typeof(Domain.Inventory.Manage.Create.Endpoint),
        typeof(Domain.Inventory.Manage.Create.Validator),
        typeof(Domain.Sales.Orders.Create.Endpoint),
        typeof(Domain.Sales.Orders.Retrieve.Endpoint),
        typeof(Domain.Shipping.EventHandlers.StartOrderProcessing),
        typeof(Inventory.Manage.Delete.Endpoint),
        typeof(Inventory.Manage.Update.Endpoint),
        typeof(Inventory.Manage.Update.Validator),
        typeof(Pipeline.PrePost.Endpoint),
        typeof(Security.ApiKey.FE_ApiKeyGet),
        typeof(Security.ApiKey.FE_ApiKeyVerify),
        typeof(Security.Claims.VerifyClaim),
        typeof(Security.Cookie.FE_CookieGetCookie),
        typeof(Security.Cookie.FE_CookieVerify),
        typeof(Security.Diagnostics.WhoAmI),
        typeof(Security.Jwt.FE_JWTGetToken),
        typeof(Security.Jwt.FE_JWTVerify),
        typeof(Security.Policies.AdminOnly.AdminPolicyVerify),
        typeof(Security.Policies.UserOnly.UserPolicyVerify),
        typeof(TestCases.CommandBusTest.EchoCommandHandler),
        typeof(TestCases.CommandBusTest.Endpoint),
        typeof(TestCases.CommandBusTest.SomeCommandHandler),
        typeof(TestCases.CommandBusTest.VoidCommandHandler),
        typeof(TestCases.CommandHandlerTest.ConcreteCmdEndpoint),
        typeof(TestCases.CommandHandlerTest.GetHandler),
        typeof(TestCases.CommandHandlerTest.GetHandlerWithoutResult),
        typeof(TestCases.CommandHandlerTest.MakeFullName),
        typeof(Uploads.Image.Save.Endpoint),
        typeof(Uploads.Image.Save.Validator),
        typeof(Uploads.Image.SaveTyped.Endpoint),
        typeof(Uploads.Image.SaveTyped.Validator),
        typeof(Versioning.Sample.EndpointV1),
        typeof(Versioning.Sample.EndpointV2),
    ];
}