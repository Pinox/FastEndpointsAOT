#pragma warning disable CS0618
#nullable enable

using FastEndpoints;
using System.Globalization;
using System.Runtime.CompilerServices;

using t0 = Admin.Login.Request;
using t1 = Binding.Multipart.UploadRequest;
using t2 = Shared.Contracts.Binding.RouteAndQueryRequest;
using t3 = int;
using t4 = Domain.Customers.CreateWithPropertiesDI.Request;
using t5 = bool;
using t6 = Domain.Customers.CreateWithPropertiesDI.Endpoint;
using t7 = Domain.Customers.Create.Request;
using t8 = Shared.Contracts.Customers.UpdateCustomerWithHeaderRequest;
using t9 = Domain.Customers.Update.Request;
using t10 = Shared.Contracts.Inventory.GetProductResponse;
using t11 = long;
using t12 = Shared.Contracts.Inventory.ListRecentInventoryResponse;
using t13 = Domain.Inventory.Manage.Create.Request;
using t14 = decimal;
using t15 = Inventory.Manage.Delete.Request;
using t16 = Inventory.Manage.Update.Request;
using t17 = Shared.Contracts.Sales.Orders.CreateOrderRequest;
using t18 = System.Guid;
using t19 = Domain.Sales.Orders.Create.Endpoint;
using t20 = Shared.Contracts.Sales.Orders.RetrieveOrderRequest;
using t21 = Shared.Contracts.Pipeline.PipelineRequest;
using t22 = Security.Claims.VerifyClaimRequest;
using t23 = Shared.Contracts.Security.WhoAmIResponse;
using t25 = Shared.Contracts.Security.ClaimKV;
using t26 = Uploads.Image.SaveTyped.Request;
using t27 = Uploads.Image.Save.Request;
using t28 = Shared.Contracts.Versioning.VersionSampleRequest;
using t29 = TestCases.CustomRequestBinder.Request;
using t31 = TestCases.CustomRequestBinder.Product;
using t32 = TestCases.DontBindAttributeTest.Request;
using t33 = TestCases.DupeParamBindingForIEnumerableProps.Request;
using t34 = double;
using t35 = TestCases.FormBindingComplexDtos.Request;
using t41 = TestCases.FormBindingComplexDtos.Book;
using t40 = TestCases.FormBindingComplexDtos.Author;
using t39 = TestCases.FormBindingComplexDtos.Address;
using t42 = TestCases.FormFileBindingTest.Request;
using t43 = TestCases.FromBodyJsonBinding.Request;
using t45 = TestCases.FromBodyJsonBinding.Product;
using t46 = TestCases.JsonArrayBindingForIEnumerableProps.Request;
using t48 = TestCases.JsonArrayBindingForIEnumerableProps.Request.Person;
using t50 = TestCases.JsonArrayBindingToListOfModels.Request;
using t51 = TestCases.QueryObjectBindingTest.Request;
using t55 = TestCases.QueryObjectBindingTest.Person;
using t54 = TestCases.QueryObjectBindingTest.NestedPerson;
using t56 = TestCases.QueryObjectWithObjectsArrayBindingTest.Request;
using t62 = TestCases.QueryObjectWithObjectsArrayBindingTest.Person;
using t61 = TestCases.QueryObjectWithObjectsArrayBindingTest.NestedPerson;
using t60 = TestCases.QueryObjectWithObjectsArrayBindingTest.ObjectInArray;
using t63 = TestCases.QueryParamBindingInEpWithoutReq.Response;
using t64 = float;
using t65 = TestCases.RouteBindingInEpWithoutReq.Response;
using t66 = TestCases.RouteBindingTest.Request;
using t68 = TestCases.RouteBindingTest.Custom;
using t72 = TestCases.RouteBindingTest.Person;
using t71 = TestCases.RouteBindingTest.NestedPerson;
using t73 = TestCases.StronglyTypedRouteParamTest.Request;
using t74 = TestCases.TypedHeaderBindingTest.MyRequest;
using t77 = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue;
using t76 = System.DateTimeOffset;
using t78 = TestCases.Endpoints.CacheBypassTest.Request;
using t79 = TestCases.DontCatchExceptions.Request;
using t80 = TestCases.EventStreamTest.Request;
using t82 = TestCases.EventStreamTest.SomeNotification;
using t83 = TestCases.HydratedQueryParamGeneratorTest.Request;
using t85 = TestCases.HydratedQueryParamGeneratorTest.Request.NestedClass;
using t87 = TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClass;
using t89 = TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClassWithToString;
using t90 = TestCases.HydratedTestUrlGeneratorTest.Request;
using t91 = TestCases.IncludedValidatorTest.Request;
using t92 = TestCases.OnBeforeAfterValidationTest.Request;
using t93 = TestCases.PlainTextRequestTest.Request;
using t94 = TestCases.TypedResultTest.Request;
using t95 = TestCases.Idempotency.Request;
using t97 = TestCases.CommandBusTest.SomeCommand;
using t99 = TestCases.CommandBusTest.EchoCommand;
using t101 = TestCases.CommandBusTest.VoidCommand;
using t103 = TestCases.CommandHandlerTest.GetFullName;
using t105 = TestCases.JobQueueTest.JobCancelTestCommand;
using t107 = TestCases.JobQueueTest.JobProgressTestCommand;
using t109 = TestCases.JobQueueTest.JobTestCommand;
using t111 = TestCases.JobQueueTest.JobWithResultTestCommand;
using t112 = TestCases.GlobalRoutePrefixOverrideTest.Request;
using t113 = TestCases.MapperTest.Request;
using t114 = TestCases.STJInfiniteRecursionTest.Response;
using t115 = TestCases.UnitTestConcurrencyTest.Request;
using t116 = TestCases.UnitTestConcurrencyTest.Endpoint;
using t117 = TestCases.GlobalGenericProcessorTest.Request;
using t118 = TestCases.PostProcessorTest.Request;
using t119 = TestCases.ProcessorAttributesTest.Request;
using t120 = TestCases.ProcessorStateTest.Request;
using t121 = TestCases.Routing.NonOptionalRouteParamTest.Request;
using t122 = TestCases.Routing.OptionalRouteParamTest.Request;
using t123 = TestCases.AntiforgeryTest.TokenResponse;
using t124 = TestCases.AntiforgeryTest.VerificationRequest;
using t125 = TestCases.MissingClaimTest.DontThrowIfMissingRequest;
using t126 = TestCases.MissingClaimTest.ThrowIfMissingRequest;
using t127 = TestCases.MissingHeaderTest.DontThrowIfMissingRequest;
using t128 = TestCases.MissingHeaderTest.ThrowIfMissingRequest;
using t129 = TestCases.RateLimitTests.Response;
using t130 = FastEndpoints.Security.TokenResponse;
using t131 = FastEndpoints.Security.TokenRequest;
using t132 = TestCases.DataAnnotationCompliant.Request;
using t134 = TestCases.DataAnnotationCompliant.NestedRequest;
using t136 = TestCases.DataAnnotationCompliant.ChildRequest;
using t137 = TestCases.PreProcessorShortWhileValidatorFails.Request;
using t138 = TestCases.PreProcessorIsRunOnValidationFailure.Request;
using t139 = TestCases.ValidationErrorTest.ArrayRequest;
using t140 = TestCases.ValidationErrorTest.DictionaryRequest;
using t141 = TestCases.ValidationErrorTest.ListInListRequest;
using t142 = TestCases.ValidationErrorTest.ListRequest;
using t143 = TestCases.ValidationErrorTest.ObjectArrayRequest;
using t145 = TestCases.ValidationErrorTest.TObject;

namespace Web;

/// <summary>
/// source generated reflection data for request dtos located in the [Web] assembly.
/// </summary>
public static class GeneratedReflection
{
    /// <summary>
    /// register source generated reflection data from [Web] with the central cache.
    /// </summary>
    public static ReflectionCache AddFromWeb(this ReflectionCache cache)
    {
        // Admin.Login.Request
        var _t0 = typeof(t0);
        cache.TryAdd(
            _t0,
            new()
            {
                ObjectFactory = () => new t0(),
                Properties = new(
                [
                     new(_t0.GetProperty(nameof(t0.email))!, new() { Setter = (dto, val) => ((t0)dto).email = (string)val! }),
                     new(_t0.GetProperty(nameof(t0.Password))!, new() { Setter = (dto, val) => ((t0)dto).Password = (string)val! }),
                ])
            });
        // Binding.Multipart.UploadRequest
        var _t1 = typeof(t1);
        cache.TryAdd(
            _t1,
            new()
            {
                ObjectFactory = () => new t1(),
                Properties = new(
                [
                     new(_t1.GetProperty(nameof(t1.Title))!, new() { Setter = (dto, val) => ((t1)dto).Title = (string?)val! }),
                     new(_t1.GetProperty(nameof(t1.Description))!, new() { Setter = (dto, val) => ((t1)dto).Description = (string?)val! }),
                     new(_t1.GetProperty(nameof(t1.File))!, new() { Setter = (dto, val) => ((t1)dto).File = (Microsoft.AspNetCore.Http.IFormFile?)val! }),
                     new(_t1.GetProperty(nameof(t1.Title))!, new() { Setter = (dto, val) => ((t1)dto).Title = (string?)val! }),
                     new(_t1.GetProperty(nameof(t1.Description))!, new() { Setter = (dto, val) => ((t1)dto).Description = (string?)val! }),
                ])
            });
        // Shared.Contracts.Binding.RouteAndQueryRequest
        var _t2 = typeof(t2);
        cache.TryAdd(
            _t2,
            new()
            {
                ObjectFactory = () => new t2(),
                Properties = new(
                [
                     new(_t2.GetProperty(nameof(t2.Page))!, new() { Setter = (dto, val) => ((t2)dto).Page = (int)val! }),
                     new(_t2.GetProperty(nameof(t2.PageSize))!, new() { Setter = (dto, val) => ((t2)dto).PageSize = (int)val! }),
                     new(_t2.GetProperty(nameof(t2.Id))!, new() { Setter = (dto, val) => ((t2)dto).Id = (string)val! }),
                ])
            });
        // int
        var _t3 = typeof(t3);
        cache.TryAdd(
            _t3,
            new()
            {
                ValueParser = input => new(t3.TryParse(input, out var result), result),
            });
        // Domain.Customers.CreateWithPropertiesDI.Request
        var _t4 = typeof(t4);
        cache.TryAdd(
            _t4,
            new()
            {
                ObjectFactory = () => new t4(),
                Properties = new(
                [
                     new(_t4.GetProperty(nameof(t4.cID))!, new() { Setter = (dto, val) => ((t4)dto).cID = (string)val! }),
                     new(_t4.GetProperty(nameof(t4.CreatedBy))!, new() { Setter = (dto, val) => ((t4)dto).CreatedBy = (string?)val! }),
                     new(_t4.GetProperty(nameof(t4.HasCreatePermission))!, new() { Setter = (dto, val) => ((t4)dto).HasCreatePermission = (bool)val! }),
                     new(_t4.GetProperty(nameof(t4.CustomerID))!, new() { Setter = (dto, val) => ((t4)dto).CustomerID = (string)val! }),
                     new(_t4.GetProperty(nameof(t4.CreatedBy))!, new() { Setter = (dto, val) => ((t4)dto).CreatedBy = (string?)val! }),
                     new(_t4.GetProperty(nameof(t4.CustomerName))!, new() { Setter = (dto, val) => ((t4)dto).CustomerName = (string?)val! }),
                     new(_t4.GetProperty(nameof(t4.PhoneNumbers))!, new() { Setter = (dto, val) => ((t4)dto).PhoneNumbers = (System.Collections.Generic.IEnumerable<string>)val! }),
                     new(_t4.GetProperty(nameof(t4.HasCreatePermission))!, new() { Setter = (dto, val) => ((t4)dto).HasCreatePermission = (bool)val! }),
                ])
            });
        // bool
        var _t5 = typeof(t5);
        cache.TryAdd(
            _t5,
            new()
            {
                ValueParser = input => new(t5.TryParse(input, out var result), result),
            });
        // Domain.Customers.CreateWithPropertiesDI.Endpoint
        var _t6 = typeof(t6);
        cache.TryAdd(
            _t6,
            new()
            {
                Properties = new(
                [
                     new(_t6.GetProperty(nameof(t6.Emailer))!, new()),
                ])
            });
        // Domain.Customers.Create.Request
        var _t7 = typeof(t7);
        cache.TryAdd(
            _t7,
            new()
            {
                ObjectFactory = () => new t7(),
                Properties = new(
                [
                     new(_t7.GetProperty(nameof(t7.cID))!, new() { Setter = (dto, val) => ((t7)dto).cID = (string)val! }),
                     new(_t7.GetProperty(nameof(t7.CreatedBy))!, new() { Setter = (dto, val) => ((t7)dto).CreatedBy = (string?)val! }),
                     new(_t7.GetProperty(nameof(t7.HasCreatePermission))!, new() { Setter = (dto, val) => ((t7)dto).HasCreatePermission = (bool)val! }),
                     new(_t7.GetProperty(nameof(t7.CustomerID))!, new() { Setter = (dto, val) => ((t7)dto).CustomerID = (string)val! }),
                     new(_t7.GetProperty(nameof(t7.CreatedBy))!, new() { Setter = (dto, val) => ((t7)dto).CreatedBy = (string?)val! }),
                     new(_t7.GetProperty(nameof(t7.CustomerName))!, new() { Setter = (dto, val) => ((t7)dto).CustomerName = (string?)val! }),
                     new(_t7.GetProperty(nameof(t7.PhoneNumbers))!, new() { Setter = (dto, val) => ((t7)dto).PhoneNumbers = (System.Collections.Generic.IEnumerable<string>)val! }),
                     new(_t7.GetProperty(nameof(t7.HasCreatePermission))!, new() { Setter = (dto, val) => ((t7)dto).HasCreatePermission = (bool)val! }),
                ])
            });
        // Shared.Contracts.Customers.UpdateCustomerWithHeaderRequest
        var _t8 = typeof(t8);
        cache.TryAdd(
            _t8,
            new()
            {
                ObjectFactory = () => new t8(default!, default!, default!, default!, default!),
                Properties = new(
                [
                     new(_t8.GetProperty(nameof(t8.CustomerID))!, new()),
                     new(_t8.GetProperty(nameof(t8.TenantID))!, new()),
                     new(_t8.GetProperty(nameof(t8.Name))!, new()),
                     new(_t8.GetProperty(nameof(t8.Age))!, new()),
                     new(_t8.GetProperty(nameof(t8.Address))!, new()),
                ])
            });
        // Domain.Customers.Update.Request
        var _t9 = typeof(t9);
        cache.TryAdd(
            _t9,
            new()
            {
                ObjectFactory = () => new t9(),
                Properties = new(
                [
                     new(_t9.GetProperty(nameof(t9.CustomerID))!, new() { Setter = (dto, val) => ((t9)dto).CustomerID = (string)val! }),
                     new(_t9.GetProperty(nameof(t9.Name))!, new() { Setter = (dto, val) => ((t9)dto).Name = (string)val! }),
                     new(_t9.GetProperty(nameof(t9.CustomerID))!, new() { Setter = (dto, val) => ((t9)dto).CustomerID = (string)val! }),
                     new(_t9.GetProperty(nameof(t9.Name))!, new() { Setter = (dto, val) => ((t9)dto).Name = (string)val! }),
                     new(_t9.GetProperty(nameof(t9.Age))!, new() { Setter = (dto, val) => ((t9)dto).Age = (int)val! }),
                     new(_t9.GetProperty(nameof(t9.Address))!, new() { Setter = (dto, val) => ((t9)dto).Address = (string)val! }),
                ])
            });
        // Shared.Contracts.Inventory.GetProductResponse
        var _t10 = typeof(t10);
        cache.TryAdd(
            _t10,
            new()
            {
                ObjectFactory = () => new t10(),
                Properties = new(
                [
                     new(_t10.GetProperty(nameof(t10.ProductID))!, new() { Setter = (dto, val) => ((t10)dto).ProductID = (string?)val! }),
                     new(_t10.GetProperty(nameof(t10.LastModified))!, new() { Setter = (dto, val) => ((t10)dto).LastModified = (long)val! }),
                ])
            });
        // long
        var _t11 = typeof(t11);
        cache.TryAdd(
            _t11,
            new()
            {
                ValueParser = input => new(t11.TryParse(input, out var result), result),
            });
        // Shared.Contracts.Inventory.ListRecentInventoryResponse
        var _t12 = typeof(t12);
        cache.TryAdd(
            _t12,
            new()
            {
                ObjectFactory = () => new t12(),
                Properties = new(
                [
                     new(_t12.GetProperty(nameof(t12.Category))!, new() { Setter = (dto, val) => ((t12)dto).Category = (string?)val! }),
                ])
            });
        // Domain.Inventory.Manage.Create.Request
        var _t13 = typeof(t13);
        cache.TryAdd(
            _t13,
            new()
            {
                ObjectFactory = () => new t13(),
                Properties = new(
                [
                     new(_t13.GetProperty(nameof(t13.UserID))!, new() { Setter = (dto, val) => ((t13)dto).UserID = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.Username))!, new() { Setter = (dto, val) => ((t13)dto).Username = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.UserID))!, new() { Setter = (dto, val) => ((t13)dto).UserID = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.Username))!, new() { Setter = (dto, val) => ((t13)dto).Username = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.Id))!, new() { Setter = (dto, val) => ((t13)dto).Id = (int)val! }),
                     new(_t13.GetProperty(nameof(t13.Name))!, new() { Setter = (dto, val) => ((t13)dto).Name = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.Description))!, new() { Setter = (dto, val) => ((t13)dto).Description = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.Price))!, new() { Setter = (dto, val) => ((t13)dto).Price = (decimal)val! }),
                     new(_t13.GetProperty(nameof(t13.QtyOnHand))!, new() { Setter = (dto, val) => ((t13)dto).QtyOnHand = (int)val! }),
                     new(_t13.GetProperty(nameof(t13.ModifiedBy))!, new() { Setter = (dto, val) => ((t13)dto).ModifiedBy = (string?)val! }),
                     new(_t13.GetProperty(nameof(t13.GenerateFullUrl))!, new() { Setter = (dto, val) => ((t13)dto).GenerateFullUrl = (bool)val! }),
                ])
            });
        // decimal
        var _t14 = typeof(t14);
        cache.TryAdd(
            _t14,
            new()
            {
                ValueParser = input => new(t14.TryParse(input, out var result), result),
            });
        // Inventory.Manage.Delete.Request
        var _t15 = typeof(t15);
        cache.TryAdd(
            _t15,
            new()
            {
                ObjectFactory = () => new t15(),
                Properties = new(
                [
                     new(_t15.GetProperty(nameof(t15.ItemID))!, new() { Setter = (dto, val) => ((t15)dto).ItemID = (string)val! }),
                     new(_t15.GetProperty(nameof(t15.ItemID))!, new() { Setter = (dto, val) => ((t15)dto).ItemID = (string)val! }),
                ])
            });
        // Inventory.Manage.Update.Request
        var _t16 = typeof(t16);
        cache.TryAdd(
            _t16,
            new()
            {
                ObjectFactory = () => new t16(),
                Properties = new(
                [
                     new(_t16.GetProperty(nameof(t16.UserID))!, new() { Setter = (dto, val) => ((t16)dto).UserID = (string?)val! }),
                     new(_t16.GetProperty(nameof(t16.UserID))!, new() { Setter = (dto, val) => ((t16)dto).UserID = (string?)val! }),
                     new(_t16.GetProperty(nameof(t16.Id))!, new() { Setter = (dto, val) => ((t16)dto).Id = (int)val! }),
                     new(_t16.GetProperty(nameof(t16.Name))!, new() { Setter = (dto, val) => ((t16)dto).Name = (string?)val! }),
                     new(_t16.GetProperty(nameof(t16.Description))!, new() { Setter = (dto, val) => ((t16)dto).Description = (string?)val! }),
                     new(_t16.GetProperty(nameof(t16.Price))!, new() { Setter = (dto, val) => ((t16)dto).Price = (decimal)val! }),
                     new(_t16.GetProperty(nameof(t16.QtyOnHand))!, new() { Setter = (dto, val) => ((t16)dto).QtyOnHand = (int)val! }),
                     new(_t16.GetProperty(nameof(t16.ModifiedBy))!, new() { Setter = (dto, val) => ((t16)dto).ModifiedBy = (string?)val! }),
                ])
            });
        // Shared.Contracts.Sales.Orders.CreateOrderRequest
        var _t17 = typeof(t17);
        cache.TryAdd(
            _t17,
            new()
            {
                ObjectFactory = () => new t17(),
                Properties = new(
                [
                     new(_t17.GetProperty(nameof(t17.CustomerID))!, new() { Setter = (dto, val) => ((t17)dto).CustomerID = (int)val! }),
                     new(_t17.GetProperty(nameof(t17.ProductID))!, new() { Setter = (dto, val) => ((t17)dto).ProductID = (int)val! }),
                     new(_t17.GetProperty(nameof(t17.Quantity))!, new() { Setter = (dto, val) => ((t17)dto).Quantity = (int)val! }),
                     new(_t17.GetProperty(nameof(t17.GuidTest))!, new() { Setter = (dto, val) => ((t17)dto).GuidTest = (System.Guid)val! }),
                ])
            });
        // System.Guid
        var _t18 = typeof(t18);
        cache.TryAdd(
            _t18,
            new()
            {
                ValueParser = input => new(t18.TryParse(input, out var result), result),
            });
        // Domain.Sales.Orders.Create.Endpoint
        var _t19 = typeof(t19);
        cache.TryAdd(
            _t19,
            new()
            {
                Properties = new(
                [
                     new(_t19.GetProperty(nameof(t19.Emailer))!, new() { Setter = (dto, val) => ((t19)dto).Emailer = (Web.Services.IEmailService)val! }),
                ])
            });
        // Shared.Contracts.Sales.Orders.RetrieveOrderRequest
        var _t20 = typeof(t20);
        cache.TryAdd(
            _t20,
            new()
            {
                ObjectFactory = () => new t20(),
                Properties = new(
                [
                     new(_t20.GetProperty(nameof(t20.TenantID))!, new() { Setter = (dto, val) => ((t20)dto).TenantID = (string)val! }),
                     new(_t20.GetProperty(nameof(t20.OrderID))!, new() { Setter = (dto, val) => ((t20)dto).OrderID = (string)val! }),
                     new(_t20.GetProperty(nameof(t20.ContentType))!, new() { Setter = (dto, val) => ((t20)dto).ContentType = (string)val! }),
                ])
            });
        // Shared.Contracts.Pipeline.PipelineRequest
        var _t21 = typeof(t21);
        cache.TryAdd(
            _t21,
            new()
            {
                ObjectFactory = () => new t21(),
                Properties = new(
                [
                     new(_t21.GetProperty(nameof(t21.Name))!, new() { Setter = (dto, val) => ((t21)dto).Name = (string)val! }),
                ])
            });
        // Security.Claims.VerifyClaimRequest
        var _t22 = typeof(t22);
        cache.TryAdd(
            _t22,
            new()
            {
                ObjectFactory = () => new t22(),
                Properties = new(
                [
                     new(_t22.GetProperty(nameof(t22.Type))!, new() { Setter = (dto, val) => ((t22)dto).Type = (string)val! }),
                     new(_t22.GetProperty(nameof(t22.Value))!, new() { Setter = (dto, val) => ((t22)dto).Value = (string?)val! }),
                ])
            });
        // Shared.Contracts.Security.WhoAmIResponse
        var _t23 = typeof(t23);
        cache.TryAdd(
            _t23,
            new()
            {
                ObjectFactory = () => new t23(),
                Properties = new(
                [
                     new(_t23.GetProperty(nameof(t23.IsAuthenticated))!, new() { Setter = (dto, val) => ((t23)dto).IsAuthenticated = (bool)val! }),
                     new(_t23.GetProperty(nameof(t23.AuthenticationType))!, new() { Setter = (dto, val) => ((t23)dto).AuthenticationType = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.Name))!, new() { Setter = (dto, val) => ((t23)dto).Name = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.NameClaimType))!, new() { Setter = (dto, val) => ((t23)dto).NameClaimType = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.RoleClaimType))!, new() { Setter = (dto, val) => ((t23)dto).RoleClaimType = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.IsAdminRole))!, new() { Setter = (dto, val) => ((t23)dto).IsAdminRole = (bool)val! }),
                     new(_t23.GetProperty(nameof(t23.Roles))!, new() { Setter = (dto, val) => ((t23)dto).Roles = (string[])val! }),
                     new(_t23.GetProperty(nameof(t23.Email))!, new() { Setter = (dto, val) => ((t23)dto).Email = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.Claims))!, new() { Setter = (dto, val) => ((t23)dto).Claims = (Shared.Contracts.Security.ClaimKV[])val! }),
                     new(_t23.GetProperty(nameof(t23.AcceptedAuthSchemes))!, new() { Setter = (dto, val) => ((t23)dto).AcceptedAuthSchemes = (string[])val! }),
                     new(_t23.GetProperty(nameof(t23.CookieExpiresUtcFormatted))!, new() { Setter = (dto, val) => ((t23)dto).CookieExpiresUtcFormatted = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.CookieExpiresUkFormatted))!, new() { Setter = (dto, val) => ((t23)dto).CookieExpiresUkFormatted = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.JwtExpiresUtcFormatted))!, new() { Setter = (dto, val) => ((t23)dto).JwtExpiresUtcFormatted = (string?)val! }),
                     new(_t23.GetProperty(nameof(t23.JwtExpiresUkFormatted))!, new() { Setter = (dto, val) => ((t23)dto).JwtExpiresUkFormatted = (string?)val! }),
                ])
            });
        // Shared.Contracts.Security.ClaimKV
        var _t25 = typeof(t25);
        cache.TryAdd(
            _t25,
            new()
            {
                ObjectFactory = () => new t25(default!, default!),
                Properties = new(
                [
                     new(_t25.GetProperty(nameof(t25.Type))!, new()),
                     new(_t25.GetProperty(nameof(t25.Value))!, new()),
                ])
            });
        // Uploads.Image.SaveTyped.Request
        var _t26 = typeof(t26);
        cache.TryAdd(
            _t26,
            new()
            {
                ObjectFactory = () => new t26(),
                Properties = new(
                [
                     new(_t26.GetProperty(nameof(t26.File1))!, new() { Setter = (dto, val) => ((t26)dto).File1 = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t26.GetProperty(nameof(t26.File2))!, new() { Setter = (dto, val) => ((t26)dto).File2 = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t26.GetProperty(nameof(t26.File3))!, new() { Setter = (dto, val) => ((t26)dto).File3 = (Microsoft.AspNetCore.Http.IFormFile?)val! }),
                     new(_t26.GetProperty(nameof(t26.File4))!, new() { Setter = (dto, val) => ((t26)dto).File4 = (Microsoft.AspNetCore.Http.IFormFile?)val! }),
                     new(_t26.GetProperty(nameof(t26.Cars))!, new() { Setter = (dto, val) => ((t26)dto).Cars = (System.Collections.Generic.IEnumerable<Microsoft.AspNetCore.Http.IFormFile>)val! }),
                     new(_t26.GetProperty(nameof(t26.Jets))!, new() { Setter = (dto, val) => ((t26)dto).Jets = (Microsoft.AspNetCore.Http.IFormFileCollection)val! }),
                     new(_t26.GetProperty(nameof(t26.GuidId))!, new() { Setter = (dto, val) => ((t26)dto).GuidId = (System.Guid)val! }),
                     new(_t26.GetProperty(nameof(t26.ID))!, new() { Setter = (dto, val) => ((t26)dto).ID = (string)val! }),
                     new(_t26.GetProperty(nameof(t26.Width))!, new() { Setter = (dto, val) => ((t26)dto).Width = (int)val! }),
                     new(_t26.GetProperty(nameof(t26.Height))!, new() { Setter = (dto, val) => ((t26)dto).Height = (int)val! }),
                ])
            });
        // Uploads.Image.Save.Request
        var _t27 = typeof(t27);
        cache.TryAdd(
            _t27,
            new()
            {
                ObjectFactory = () => new t27(),
                Properties = new(
                [
                     new(_t27.GetProperty(nameof(t27.ID))!, new() { Setter = (dto, val) => ((t27)dto).ID = (string)val! }),
                     new(_t27.GetProperty(nameof(t27.Width))!, new() { Setter = (dto, val) => ((t27)dto).Width = (int)val! }),
                     new(_t27.GetProperty(nameof(t27.Height))!, new() { Setter = (dto, val) => ((t27)dto).Height = (int)val! }),
                ])
            });
        // Shared.Contracts.Versioning.VersionSampleRequest
        var _t28 = typeof(t28);
        cache.TryAdd(
            _t28,
            new()
            {
                ObjectFactory = () => new t28(),
                Properties = new(
                [
                     new(_t28.GetProperty(nameof(t28.Id))!, new() { Setter = (dto, val) => ((t28)dto).Id = (int)val! }),
                ])
            });
        // TestCases.CustomRequestBinder.Request
        var _t29 = typeof(t29);
        cache.TryAdd(
            _t29,
            new()
            {
                ObjectFactory = () => new t29(),
                Properties = new(
                [
                     new(_t29.GetProperty(nameof(t29.Id))!, new() { Setter = (dto, val) => ((t29)dto).Id = (string)val! }),
                     new(_t29.GetProperty(nameof(t29.Product))!, new() { Setter = (dto, val) => ((t29)dto).Product = (TestCases.CustomRequestBinder.Product?)val! }),
                     new(_t29.GetProperty(nameof(t29.CustomerID))!, new() { Setter = (dto, val) => ((t29)dto).CustomerID = (string)val! }),
                ])
            });
        // TestCases.CustomRequestBinder.Product
        var _t31 = typeof(t31);
        cache.TryAdd(
            _t31,
            new()
            {
                ObjectFactory = () => new t31(),
                Properties = new(
                [
                     new(_t31.GetProperty(nameof(t31.Id))!, new() { Setter = (dto, val) => ((t31)dto).Id = (int)val! }),
                     new(_t31.GetProperty(nameof(t31.Name))!, new() { Setter = (dto, val) => ((t31)dto).Name = (string)val! }),
                     new(_t31.GetProperty(nameof(t31.Price))!, new() { Setter = (dto, val) => ((t31)dto).Price = (decimal)val! }),
                ])
            });
        // TestCases.DontBindAttributeTest.Request
        var _t32 = typeof(t32);
        cache.TryAdd(
            _t32,
            new()
            {
                ObjectFactory = () => new t32(),
                Properties = new(
                [
                     new(_t32.GetProperty(nameof(t32.Id))!, new() { Setter = (dto, val) => ((t32)dto).Id = (int)val! }),
                     new(_t32.GetProperty(nameof(t32.Name))!, new() { Setter = (dto, val) => ((t32)dto).Name = (string)val! }),
                ])
            });
        // TestCases.DupeParamBindingForIEnumerableProps.Request
        var _t33 = typeof(t33);
        cache.TryAdd(
            _t33,
            new()
            {
                ObjectFactory = () => new t33(),
                Properties = new(
                [
                     new(_t33.GetProperty(nameof(t33.Strings))!, new() { Setter = (dto, val) => ((t33)dto).Strings = (string[])val! }),
                     new(_t33.GetProperty(nameof(t33.MoreStrings))!, new() { Setter = (dto, val) => ((t33)dto).MoreStrings = (string[])val! }),
                     new(_t33.GetProperty(nameof(t33.Doubles))!, new() { Setter = (dto, val) => ((t33)dto).Doubles = (double[])val! }),
                     new(_t33.GetProperty(nameof(t33.Ints))!, new() { Setter = (dto, val) => ((t33)dto).Ints = (System.Collections.Generic.IEnumerable<int>)val! }),
                     new(_t33.GetProperty(nameof(t33.Guids))!, new() { Setter = (dto, val) => ((t33)dto).Guids = (System.Collections.Generic.List<System.Guid>)val! }),
                     new(_t33.GetProperty(nameof(t33.Dates))!, new() { Setter = (dto, val) => ((t33)dto).Dates = (System.Collections.Generic.ICollection<System.DateTime>)val! }),
                     new(_t33.GetProperty(nameof(t33.Persons))!, new() { Setter = (dto, val) => ((t33)dto).Persons = (System.Collections.Generic.IEnumerable<TestCases.DupeParamBindingForIEnumerableProps.Request.Person>)val! }),
                ])
            });
        // double
        var _t34 = typeof(t34);
        cache.TryAdd(
            _t34,
            new()
            {
                ValueParser = input => new(t34.TryParse(input, out var result), result),
            });
        // TestCases.FormBindingComplexDtos.Request
        var _t35 = typeof(t35);
        cache.TryAdd(
            _t35,
            new()
            {
                ObjectFactory = () => new t35(),
                Properties = new(
                [
                     new(_t35.GetProperty(nameof(t35.Book))!, new() { Setter = (dto, val) => ((t35)dto).Book = (TestCases.FormBindingComplexDtos.Book)val! }),
                ])
            });
        // TestCases.FormBindingComplexDtos.Book
        var _t41 = typeof(t41);
        cache.TryAdd(
            _t41,
            new()
            {
                ObjectFactory = () => new t41(),
                Properties = new(
                [
                     new(_t41.GetProperty(nameof(t41.Title))!, new() { Setter = (dto, val) => ((t41)dto).Title = (string)val! }),
                     new(_t41.GetProperty(nameof(t41.CoverImage))!, new() { Setter = (dto, val) => ((t41)dto).CoverImage = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t41.GetProperty(nameof(t41.SourceFiles))!, new() { Setter = (dto, val) => ((t41)dto).SourceFiles = (Microsoft.AspNetCore.Http.IFormFileCollection)val! }),
                     new(_t41.GetProperty(nameof(t41.MainAuthor))!, new() { Setter = (dto, val) => ((t41)dto).MainAuthor = (TestCases.FormBindingComplexDtos.Author)val! }),
                     new(_t41.GetProperty(nameof(t41.CoAuthors))!, new() { Setter = (dto, val) => ((t41)dto).CoAuthors = (System.Collections.Generic.List<TestCases.FormBindingComplexDtos.Author>)val! }),
                     new(_t41.GetProperty(nameof(t41.BarCodes))!, new() { Setter = (dto, val) => ((t41)dto).BarCodes = (System.Collections.Generic.IEnumerable<int>)val! }),
                ])
            });
        // TestCases.FormBindingComplexDtos.Author
        var _t40 = typeof(t40);
        cache.TryAdd(
            _t40,
            new()
            {
                ObjectFactory = () => new t40(),
                Properties = new(
                [
                     new(_t40.GetProperty(nameof(t40.Name))!, new() { Setter = (dto, val) => ((t40)dto).Name = (string)val! }),
                     new(_t40.GetProperty(nameof(t40.ProfileImage))!, new() { Setter = (dto, val) => ((t40)dto).ProfileImage = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t40.GetProperty(nameof(t40.DocumentFiles))!, new() { Setter = (dto, val) => ((t40)dto).DocumentFiles = (Microsoft.AspNetCore.Http.IFormFileCollection)val! }),
                     new(_t40.GetProperty(nameof(t40.MainAddress))!, new() { Setter = (dto, val) => ((t40)dto).MainAddress = (TestCases.FormBindingComplexDtos.Address)val! }),
                     new(_t40.GetProperty(nameof(t40.OtherAddresses))!, new() { Setter = (dto, val) => ((t40)dto).OtherAddresses = (System.Collections.Generic.List<TestCases.FormBindingComplexDtos.Address>)val! }),
                ])
            });
        // TestCases.FormBindingComplexDtos.Address
        var _t39 = typeof(t39);
        cache.TryAdd(
            _t39,
            new()
            {
                ObjectFactory = () => new t39(),
                Properties = new(
                [
                     new(_t39.GetProperty(nameof(t39.Street))!, new() { Setter = (dto, val) => ((t39)dto).Street = (string)val! }),
                     new(_t39.GetProperty(nameof(t39.MainImage))!, new() { Setter = (dto, val) => ((t39)dto).MainImage = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t39.GetProperty(nameof(t39.AlternativeImages))!, new() { Setter = (dto, val) => ((t39)dto).AlternativeImages = (System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile>)val! }),
                ])
            });
        // TestCases.FormFileBindingTest.Request
        var _t42 = typeof(t42);
        cache.TryAdd(
            _t42,
            new()
            {
                ObjectFactory = () => new t42(),
                Properties = new(
                [
                     new(_t42.GetProperty(nameof(t42.ID))!, new() { Setter = (dto, val) => ((t42)dto).ID = (string)val! }),
                     new(_t42.GetProperty(nameof(t42.Width))!, new() { Setter = (dto, val) => ((t42)dto).Width = (int)val! }),
                     new(_t42.GetProperty(nameof(t42.Height))!, new() { Setter = (dto, val) => ((t42)dto).Height = (int)val! }),
                     new(_t42.GetProperty(nameof(t42.File1))!, new() { Setter = (dto, val) => ((t42)dto).File1 = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t42.GetProperty(nameof(t42.File2))!, new() { Setter = (dto, val) => ((t42)dto).File2 = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t42.GetProperty(nameof(t42.Cars))!, new() { Setter = (dto, val) => ((t42)dto).Cars = (System.Collections.Generic.IEnumerable<Microsoft.AspNetCore.Http.IFormFile>)val! }),
                     new(_t42.GetProperty(nameof(t42.Jets))!, new() { Setter = (dto, val) => ((t42)dto).Jets = (Microsoft.AspNetCore.Http.IFormFileCollection)val! }),
                ])
            });
        // TestCases.FromBodyJsonBinding.Request
        var _t43 = typeof(t43);
        cache.TryAdd(
            _t43,
            new()
            {
                ObjectFactory = () => new t43(),
                Properties = new(
                [
                     new(_t43.GetProperty(nameof(t43.Id))!, new() { Setter = (dto, val) => ((t43)dto).Id = (int)val! }),
                     new(_t43.GetProperty(nameof(t43.Product))!, new() { Setter = (dto, val) => ((t43)dto).Product = (TestCases.FromBodyJsonBinding.Product)val! }),
                     new(_t43.GetProperty(nameof(t43.CustomerID))!, new() { Setter = (dto, val) => ((t43)dto).CustomerID = (int)val! }),
                ])
            });
        // TestCases.FromBodyJsonBinding.Product
        var _t45 = typeof(t45);
        cache.TryAdd(
            _t45,
            new()
            {
                ObjectFactory = () => new t45(),
                Properties = new(
                [
                     new(_t45.GetProperty(nameof(t45.Id))!, new() { Setter = (dto, val) => ((t45)dto).Id = (int)val! }),
                     new(_t45.GetProperty(nameof(t45.Name))!, new() { Setter = (dto, val) => ((t45)dto).Name = (string)val! }),
                     new(_t45.GetProperty(nameof(t45.Price))!, new() { Setter = (dto, val) => ((t45)dto).Price = (decimal)val! }),
                ])
            });
        // TestCases.JsonArrayBindingForIEnumerableProps.Request
        var _t46 = typeof(t46);
        cache.TryAdd(
            _t46,
            new()
            {
                ObjectFactory = () => new t46(),
                Properties = new(
                [
                     new(_t46.GetProperty(nameof(t46.Doubles))!, new() { Setter = (dto, val) => ((t46)dto).Doubles = (double[])val! }),
                     new(_t46.GetProperty(nameof(t46.Ints))!, new() { Setter = (dto, val) => ((t46)dto).Ints = (System.Collections.Generic.IEnumerable<int>)val! }),
                     new(_t46.GetProperty(nameof(t46.Guids))!, new() { Setter = (dto, val) => ((t46)dto).Guids = (System.Collections.Generic.List<System.Guid>)val! }),
                     new(_t46.GetProperty(nameof(t46.Dates))!, new() { Setter = (dto, val) => ((t46)dto).Dates = (System.Collections.Generic.ICollection<System.DateTime>)val! }),
                     new(_t46.GetProperty(nameof(t46.Steven))!, new() { Setter = (dto, val) => ((t46)dto).Steven = (TestCases.JsonArrayBindingForIEnumerableProps.Request.Person)val! }),
                     new(_t46.GetProperty(nameof(t46.Dict))!, new() { Setter = (dto, val) => ((t46)dto).Dict = (System.Collections.Generic.Dictionary<string, string>)val! }),
                ])
            });
        // TestCases.JsonArrayBindingForIEnumerableProps.Request.Person
        var _t48 = typeof(t48);
        cache.TryAdd(
            _t48,
            new()
            {
                ObjectFactory = () => new t48(),
                Properties = new(
                [
                     new(_t48.GetProperty(nameof(t48.Age))!, new() { Setter = (dto, val) => ((t48)dto).Age = (int)val! }),
                     new(_t48.GetProperty(nameof(t48.Name))!, new() { Setter = (dto, val) => ((t48)dto).Name = (string)val! }),
                ])
            });
        // TestCases.JsonArrayBindingToListOfModels.Request
        var _t50 = typeof(t50);
        cache.TryAdd(
            _t50,
            new()
            {
                ObjectFactory = () => new t50(),
                Properties = new(
                [
                     new(_t50.GetProperty(nameof(t50.Name))!, new() { Setter = (dto, val) => ((t50)dto).Name = (string)val! }),
                ])
            });
        // TestCases.QueryObjectBindingTest.Request
        var _t51 = typeof(t51);
        cache.TryAdd(
            _t51,
            new()
            {
                ObjectFactory = () => new t51(),
                Properties = new(
                [
                     new(_t51.GetProperty(nameof(t51.String))!, new() { Setter = (dto, val) => ((t51)dto).String = (string)val! }),
                     new(_t51.GetProperty(nameof(t51.Bool))!, new() { Setter = (dto, val) => ((t51)dto).Bool = (bool)val! }),
                     new(_t51.GetProperty(nameof(t51.Int))!, new() { Setter = (dto, val) => ((t51)dto).Int = (int?)val! }),
                     new(_t51.GetProperty(nameof(t51.Long))!, new() { Setter = (dto, val) => ((t51)dto).Long = (long)val! }),
                     new(_t51.GetProperty(nameof(t51.Double))!, new() { Setter = (dto, val) => ((t51)dto).Double = (double)val! }),
                     new(_t51.GetProperty(nameof(t51.Enum))!, new() { Setter = (dto, val) => ((t51)dto).Enum = (System.DayOfWeek)val! }),
                     new(_t51.GetProperty(nameof(t51.Person))!, new() { Setter = (dto, val) => ((t51)dto).Person = (TestCases.QueryObjectBindingTest.Person)val! }),
                ])
            });
        // TestCases.QueryObjectBindingTest.Person
        var _t55 = typeof(t55);
        cache.TryAdd(
            _t55,
            new()
            {
                ObjectFactory = () => new t55(),
                Properties = new(
                [
                     new(_t55.GetProperty(nameof(t55.Id))!, new() { Setter = (dto, val) => ((t55)dto).Id = (System.Guid)val! }),
                     new(_t55.GetProperty(nameof(t55.Name))!, new() { Setter = (dto, val) => ((t55)dto).Name = (string)val! }),
                     new(_t55.GetProperty(nameof(t55.Age))!, new() { Setter = (dto, val) => ((t55)dto).Age = (int)val! }),
                     new(_t55.GetProperty(nameof(t55.Child))!, new() { Setter = (dto, val) => ((t55)dto).Child = (TestCases.QueryObjectBindingTest.NestedPerson)val! }),
                     new(_t55.GetProperty(nameof(t55.Numbers))!, new() { Setter = (dto, val) => ((t55)dto).Numbers = (System.Collections.Generic.List<int>)val! }),
                     new(_t55.GetProperty(nameof(t55.FavoriteDay))!, new() { Setter = (dto, val) => ((t55)dto).FavoriteDay = (System.DayOfWeek)val! }),
                     new(_t55.GetProperty(nameof(t55.ByteEnum))!, new() { Setter = (dto, val) => ((t55)dto).ByteEnum = (TestCases.QueryObjectBindingTest.ByteEnum)val! }),
                     new(_t55.GetProperty(nameof(t55.IsHidden))!, new() { Setter = (dto, val) => ((t55)dto).IsHidden = (bool)val! }),
                ])
            });
        // TestCases.QueryObjectBindingTest.NestedPerson
        var _t54 = typeof(t54);
        cache.TryAdd(
            _t54,
            new()
            {
                ObjectFactory = () => new t54(),
                Properties = new(
                [
                     new(_t54.GetProperty(nameof(t54.Id))!, new() { Setter = (dto, val) => ((t54)dto).Id = (System.Guid)val! }),
                     new(_t54.GetProperty(nameof(t54.Name))!, new() { Setter = (dto, val) => ((t54)dto).Name = (string)val! }),
                     new(_t54.GetProperty(nameof(t54.Age))!, new() { Setter = (dto, val) => ((t54)dto).Age = (int)val! }),
                     new(_t54.GetProperty(nameof(t54.Strings))!, new() { Setter = (dto, val) => ((t54)dto).Strings = (System.Collections.Generic.List<string>)val! }),
                     new(_t54.GetProperty(nameof(t54.FavoriteDays))!, new() { Setter = (dto, val) => ((t54)dto).FavoriteDays = (System.Collections.Generic.List<System.DayOfWeek>)val! }),
                     new(_t54.GetProperty(nameof(t54.IsHidden))!, new() { Setter = (dto, val) => ((t54)dto).IsHidden = (bool)val! }),
                ])
            });
        // TestCases.QueryObjectWithObjectsArrayBindingTest.Request
        var _t56 = typeof(t56);
        cache.TryAdd(
            _t56,
            new()
            {
                ObjectFactory = () => new t56(),
                Properties = new(
                [
                     new(_t56.GetProperty(nameof(t56.Person))!, new() { Setter = (dto, val) => ((t56)dto).Person = (TestCases.QueryObjectWithObjectsArrayBindingTest.Person)val! }),
                ])
            });
        // TestCases.QueryObjectWithObjectsArrayBindingTest.Person
        var _t62 = typeof(t62);
        cache.TryAdd(
            _t62,
            new()
            {
                ObjectFactory = () => new t62(),
                Properties = new(
                [
                     new(_t62.GetProperty(nameof(t62.Child))!, new() { Setter = (dto, val) => ((t62)dto).Child = (TestCases.QueryObjectWithObjectsArrayBindingTest.NestedPerson)val! }),
                     new(_t62.GetProperty(nameof(t62.Objects))!, new() { Setter = (dto, val) => ((t62)dto).Objects = (System.Collections.Generic.List<TestCases.QueryObjectWithObjectsArrayBindingTest.ObjectInArray>)val! }),
                ])
            });
        // TestCases.QueryObjectWithObjectsArrayBindingTest.NestedPerson
        var _t61 = typeof(t61);
        cache.TryAdd(
            _t61,
            new()
            {
                ObjectFactory = () => new t61(),
                Properties = new(
                [
                     new(_t61.GetProperty(nameof(t61.Objects))!, new() { Setter = (dto, val) => ((t61)dto).Objects = (System.Collections.Generic.List<TestCases.QueryObjectWithObjectsArrayBindingTest.ObjectInArray>)val! }),
                ])
            });
        // TestCases.QueryObjectWithObjectsArrayBindingTest.ObjectInArray
        var _t60 = typeof(t60);
        cache.TryAdd(
            _t60,
            new()
            {
                ObjectFactory = () => new t60(),
                Properties = new(
                [
                     new(_t60.GetProperty(nameof(t60.String))!, new() { Setter = (dto, val) => ((t60)dto).String = (string)val! }),
                     new(_t60.GetProperty(nameof(t60.Bool))!, new() { Setter = (dto, val) => ((t60)dto).Bool = (bool)val! }),
                     new(_t60.GetProperty(nameof(t60.Int))!, new() { Setter = (dto, val) => ((t60)dto).Int = (int?)val! }),
                     new(_t60.GetProperty(nameof(t60.Long))!, new() { Setter = (dto, val) => ((t60)dto).Long = (long)val! }),
                     new(_t60.GetProperty(nameof(t60.Double))!, new() { Setter = (dto, val) => ((t60)dto).Double = (double)val! }),
                     new(_t60.GetProperty(nameof(t60.Enum))!, new() { Setter = (dto, val) => ((t60)dto).Enum = (System.DayOfWeek)val! }),
                ])
            });
        // TestCases.QueryParamBindingInEpWithoutReq.Response
        var _t63 = typeof(t63);
        cache.TryAdd(
            _t63,
            new()
            {
                ObjectFactory = () => new t63(),
                Properties = new(
                [
                     new(_t63.GetProperty(nameof(t63.CustomerID))!, new() { Setter = (dto, val) => ((t63)dto).CustomerID = (int)val! }),
                     new(_t63.GetProperty(nameof(t63.OtherID))!, new() { Setter = (dto, val) => ((t63)dto).OtherID = (int?)val! }),
                     new(_t63.GetProperty(nameof(t63.Doubles))!, new() { Setter = (dto, val) => ((t63)dto).Doubles = (double[])val! }),
                     new(_t63.GetProperty(nameof(t63.Ints))!, new() { Setter = (dto, val) => ((t63)dto).Ints = (System.Collections.Generic.IEnumerable<int>)val! }),
                     new(_t63.GetProperty(nameof(t63.Guids))!, new() { Setter = (dto, val) => ((t63)dto).Guids = (System.Collections.Generic.List<System.Guid>)val! }),
                     new(_t63.GetProperty(nameof(t63.Floaty))!, new() { Setter = (dto, val) => ((t63)dto).Floaty = (float)val! }),
                ])
            });
        // float
        var _t64 = typeof(t64);
        cache.TryAdd(
            _t64,
            new()
            {
                ValueParser = input => new(t64.TryParse(input, out var result), result),
            });
        // TestCases.RouteBindingInEpWithoutReq.Response
        var _t65 = typeof(t65);
        cache.TryAdd(
            _t65,
            new()
            {
                ObjectFactory = () => new t65(),
                Properties = new(
                [
                     new(_t65.GetProperty(nameof(t65.CustomerID))!, new() { Setter = (dto, val) => ((t65)dto).CustomerID = (int)val! }),
                     new(_t65.GetProperty(nameof(t65.OtherID))!, new() { Setter = (dto, val) => ((t65)dto).OtherID = (int?)val! }),
                ])
            });
        // TestCases.RouteBindingTest.Request
        var _t66 = typeof(t66);
        cache.TryAdd(
            _t66,
            new()
            {
                ObjectFactory = () => new t66(),
                Properties = new(
                [
                     new(_t66.GetProperty(nameof(t66.String))!, new() { Setter = (dto, val) => ((t66)dto).String = (string)val! }),
                     new(_t66.GetProperty(nameof(t66.Bool))!, new() { Setter = (dto, val) => ((t66)dto).Bool = (bool)val! }),
                     new(_t66.GetProperty(nameof(t66.Int))!, new() { Setter = (dto, val) => ((t66)dto).Int = (int?)val! }),
                     new(_t66.GetProperty(nameof(t66.Long))!, new() { Setter = (dto, val) => ((t66)dto).Long = (long)val! }),
                     new(_t66.GetProperty(nameof(t66.Double))!, new() { Setter = (dto, val) => ((t66)dto).Double = (double)val! }),
                     new(_t66.GetProperty(nameof(t66.Url))!, new() { Setter = (dto, val) => ((t66)dto).Url = (System.Uri?)val! }),
                     new(_t66.GetProperty(nameof(t66.Custom))!, new() { Setter = (dto, val) => ((t66)dto).Custom = (TestCases.RouteBindingTest.Custom)val! }),
                     new(_t66.GetProperty(nameof(t66.CustomList))!, new() { Setter = (dto, val) => ((t66)dto).CustomList = (TestCases.RouteBindingTest.CustomList)val! }),
                     new(_t66.GetProperty(nameof(t66.DecimalNumber))!, new() { Setter = (dto, val) => ((t66)dto).DecimalNumber = (decimal)val! }),
                     new(_t66.GetProperty(nameof(t66.Blank))!, new() { Setter = (dto, val) => ((t66)dto).Blank = (int?)val! }),
                     new(_t66.GetProperty(nameof(t66.Person))!, new() { Setter = (dto, val) => ((t66)dto).Person = (TestCases.RouteBindingTest.Person)val! }),
                     new(_t66.GetProperty(nameof(t66.StringOverride))!, new() { Setter = (dto, val) => ((t66)dto).StringOverride = (long)val! }),
                     new(_t66.GetProperty(nameof(t66.StringOverrideNullable))!, new() { Setter = (dto, val) => ((t66)dto).StringOverrideNullable = (long)val! }),
                     new(_t66.GetProperty(nameof(t66.FromBody))!, new() { Setter = (dto, val) => ((t66)dto).FromBody = (string)val! }),
                ])
            });
        // TestCases.RouteBindingTest.Custom
        var _t68 = typeof(t68);
        cache.TryAdd(
            _t68,
            new()
            {
                ObjectFactory = () => new t68(),
                ValueParser = input => new(t68.TryParse(input, out var result), result),
                Properties = new(
                [
                     new(_t68.GetProperty(nameof(t68.Value))!, new() { Setter = (dto, val) => ((t68)dto).Value = (int)val! }),
                ])
            });
        // TestCases.RouteBindingTest.Person
        var _t72 = typeof(t72);
        cache.TryAdd(
            _t72,
            new()
            {
                ObjectFactory = () => new t72(),
                Properties = new(
                [
                     new(_t72.GetProperty(nameof(t72.Id))!, new() { Setter = (dto, val) => ((t72)dto).Id = (System.Guid)val! }),
                     new(_t72.GetProperty(nameof(t72.Name))!, new() { Setter = (dto, val) => ((t72)dto).Name = (string)val! }),
                     new(_t72.GetProperty(nameof(t72.Age))!, new() { Setter = (dto, val) => ((t72)dto).Age = (int)val! }),
                     new(_t72.GetProperty(nameof(t72.Child))!, new() { Setter = (dto, val) => ((t72)dto).Child = (TestCases.RouteBindingTest.NestedPerson)val! }),
                     new(_t72.GetProperty(nameof(t72.Numbers))!, new() { Setter = (dto, val) => ((t72)dto).Numbers = (System.Collections.Generic.List<int>)val! }),
                ])
            });
        // TestCases.RouteBindingTest.NestedPerson
        var _t71 = typeof(t71);
        cache.TryAdd(
            _t71,
            new()
            {
                ObjectFactory = () => new t71(),
                Properties = new(
                [
                     new(_t71.GetProperty(nameof(t71.Id))!, new() { Setter = (dto, val) => ((t71)dto).Id = (System.Guid)val! }),
                     new(_t71.GetProperty(nameof(t71.Name))!, new() { Setter = (dto, val) => ((t71)dto).Name = (string)val! }),
                     new(_t71.GetProperty(nameof(t71.Age))!, new() { Setter = (dto, val) => ((t71)dto).Age = (int)val! }),
                     new(_t71.GetProperty(nameof(t71.Strings))!, new() { Setter = (dto, val) => ((t71)dto).Strings = (System.Collections.Generic.List<string>)val! }),
                ])
            });
        // TestCases.StronglyTypedRouteParamTest.Request
        var _t73 = typeof(t73);
        cache.TryAdd(
            _t73,
            new()
            {
                ObjectFactory = () => new t73(),
                Properties = new(
                [
                     new(_t73.GetProperty(nameof(t73.Uid))!, new() { Setter = (dto, val) => ((t73)dto).Uid = (string)val! }),
                     new(_t73.GetProperty(nameof(t73.Name))!, new() { Setter = (dto, val) => ((t73)dto).Name = (string)val! }),
                ])
            });
        // TestCases.TypedHeaderBindingTest.MyRequest
        var _t74 = typeof(t74);
        cache.TryAdd(
            _t74,
            new()
            {
                ObjectFactory = () => new t74(),
                Properties = new(
                [
                     new(_t74.GetProperty(nameof(t74.Disposition))!, new() { Setter = (dto, val) => ((t74)dto).Disposition = (Microsoft.Net.Http.Headers.ContentDispositionHeaderValue)val! }),
                     new(_t74.GetProperty(nameof(t74.Content))!, new() { Setter = (dto, val) => ((t74)dto).Content = (string)val! }),
                ])
            });
        // Microsoft.Net.Http.Headers.ContentDispositionHeaderValue
        var _t77 = typeof(t77);
        cache.TryAdd(
            _t77,
            new()
            {
                ObjectFactory = () => new t77(default!),
                Properties = new(
                [
                     new(_t77.GetProperty(nameof(t77.DispositionType))!, new() { Setter = (dto, val) => ((t77)dto).DispositionType = (Microsoft.Extensions.Primitives.StringSegment)val! }),
                     new(_t77.GetProperty(nameof(t77.Name))!, new() { Setter = (dto, val) => ((t77)dto).Name = (Microsoft.Extensions.Primitives.StringSegment)val! }),
                     new(_t77.GetProperty(nameof(t77.FileName))!, new() { Setter = (dto, val) => ((t77)dto).FileName = (Microsoft.Extensions.Primitives.StringSegment)val! }),
                     new(_t77.GetProperty(nameof(t77.FileNameStar))!, new() { Setter = (dto, val) => ((t77)dto).FileNameStar = (Microsoft.Extensions.Primitives.StringSegment)val! }),
                     new(_t77.GetProperty(nameof(t77.CreationDate))!, new() { Setter = (dto, val) => ((t77)dto).CreationDate = (System.DateTimeOffset?)val! }),
                     new(_t77.GetProperty(nameof(t77.ModificationDate))!, new() { Setter = (dto, val) => ((t77)dto).ModificationDate = (System.DateTimeOffset?)val! }),
                     new(_t77.GetProperty(nameof(t77.ReadDate))!, new() { Setter = (dto, val) => ((t77)dto).ReadDate = (System.DateTimeOffset?)val! }),
                     new(_t77.GetProperty(nameof(t77.Size))!, new() { Setter = (dto, val) => ((t77)dto).Size = (long?)val! }),
                ])
            });
        // System.DateTimeOffset
        var _t76 = typeof(t76);
        cache.TryAdd(
            _t76,
            new()
            {
                ValueParser = input => new(t76.TryParse(input, out var result), result),
            });
        // TestCases.Endpoints.CacheBypassTest.Request
        var _t78 = typeof(t78);
        cache.TryAdd(
            _t78,
            new()
            {
                ObjectFactory = () => new t78(),
                Properties = new(
                [
                     new(_t78.GetProperty(nameof(t78.Id))!, new() { Setter = (dto, val) => ((t78)dto).Id = (System.Guid)val! }),
                ])
            });
        // TestCases.DontCatchExceptions.Request
        var _t79 = typeof(t79);
        cache.TryAdd(
            _t79,
            new()
            {
                ObjectFactory = () => new t79(),
                Properties = new(
                [
                     new(_t79.GetProperty(nameof(t79.Number))!, new() { Setter = (dto, val) => ((t79)dto).Number = (int)val! }),
                ])
            });
        // TestCases.EventStreamTest.Request
        var _t80 = typeof(t80);
        cache.TryAdd(
            _t80,
            new()
            {
                ObjectFactory = () => new t80(default!, default!),
                Properties = new(
                [
                     new(_t80.GetProperty(nameof(t80.EventName))!, new()),
                     new(_t80.GetProperty(nameof(t80.Notifications))!, new()),
                ])
            });
        // TestCases.EventStreamTest.SomeNotification
        var _t82 = typeof(t82);
        cache.TryAdd(
            _t82,
            new()
            {
                ObjectFactory = () => new t82(default!),
                Properties = new(
                [
                     new(_t82.GetProperty(nameof(t82.Name))!, new()),
                ])
            });
        // TestCases.HydratedQueryParamGeneratorTest.Request
        var _t83 = typeof(t83);
        cache.TryAdd(
            _t83,
            new()
            {
                ObjectFactory = () => new t83(),
                Properties = new(
                [
                     new(_t83.GetProperty(nameof(t83.Nested))!, new() { Setter = (dto, val) => ((t83)dto).Nested = (TestCases.HydratedQueryParamGeneratorTest.Request.NestedClass)val! }),
                     new(_t83.GetProperty(nameof(t83.Guids))!, new() { Setter = (dto, val) => ((t83)dto).Guids = (System.Collections.Generic.List<System.Guid>)val! }),
                     new(_t83.GetProperty(nameof(t83.Some))!, new() { Setter = (dto, val) => ((t83)dto).Some = (string?)val! }),
                     new(_t83.GetProperty(nameof(t83.ComplexId))!, new() { Setter = (dto, val) => ((t83)dto).ComplexId = (TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClass)val! }),
                     new(_t83.GetProperty(nameof(t83.ComplexIdString))!, new() { Setter = (dto, val) => ((t83)dto).ComplexIdString = (TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClassWithToString)val! }),
                ])
            });
        // TestCases.HydratedQueryParamGeneratorTest.Request.NestedClass
        var _t85 = typeof(t85);
        cache.TryAdd(
            _t85,
            new()
            {
                ObjectFactory = () => new t85(default!, default!),
                Properties = new(
                [
                     new(_t85.GetProperty(nameof(t85.First))!, new()),
                     new(_t85.GetProperty(nameof(t85.Last))!, new()),
                ])
            });
        // TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClass
        var _t87 = typeof(t87);
        cache.TryAdd(
            _t87,
            new()
            {
                ObjectFactory = () => new t87(),
                Properties = new(
                [
                     new(_t87.GetProperty(nameof(t87.Number1))!, new() { Setter = (dto, val) => ((t87)dto).Number1 = (int)val! }),
                     new(_t87.GetProperty(nameof(t87.Number2))!, new() { Setter = (dto, val) => ((t87)dto).Number2 = (int)val! }),
                ])
            });
        // TestCases.HydratedQueryParamGeneratorTest.Request.ComplexIdClassWithToString
        var _t89 = typeof(t89);
        cache.TryAdd(
            _t89,
            new()
            {
                ObjectFactory = () => new t89(),
                ValueParser = input => new(t89.TryParse(input, out var result), result),
                Properties = new(
                [
                     new(_t89.GetProperty(nameof(t89.Number1))!, new() { Setter = (dto, val) => ((t89)dto).Number1 = (int)val! }),
                     new(_t89.GetProperty(nameof(t89.Number2))!, new() { Setter = (dto, val) => ((t89)dto).Number2 = (int)val! }),
                ])
            });
        // TestCases.HydratedTestUrlGeneratorTest.Request
        var _t90 = typeof(t90);
        cache.TryAdd(
            _t90,
            new()
            {
                ObjectFactory = () => new t90(),
                Properties = new(
                [
                     new(_t90.GetProperty(nameof(t90.Id))!, new() { Setter = (dto, val) => ((t90)dto).Id = (int)val! }),
                     new(_t90.GetProperty(nameof(t90.Guid))!, new() { Setter = (dto, val) => ((t90)dto).Guid = (System.Guid)val! }),
                     new(_t90.GetProperty(nameof(t90.String))!, new() { Setter = (dto, val) => ((t90)dto).String = (string)val! }),
                     new(_t90.GetProperty(nameof(t90.NullableString))!, new() { Setter = (dto, val) => ((t90)dto).NullableString = (string?)val! }),
                     new(_t90.GetProperty(nameof(t90.FromClaim))!, new() { Setter = (dto, val) => ((t90)dto).FromClaim = (string)val! }),
                     new(_t90.GetProperty(nameof(t90.FromHeader))!, new() { Setter = (dto, val) => ((t90)dto).FromHeader = (string)val! }),
                     new(_t90.GetProperty(nameof(t90.HasPermission))!, new() { Setter = (dto, val) => ((t90)dto).HasPermission = (bool?)val! }),
                ])
            });
        // TestCases.IncludedValidatorTest.Request
        var _t91 = typeof(t91);
        cache.TryAdd(
            _t91,
            new()
            {
                ObjectFactory = () => new t91(),
                Properties = new(
                [
                     new(_t91.GetProperty(nameof(t91.Name))!, new() { Setter = (dto, val) => ((t91)dto).Name = (string)val! }),
                     new(_t91.GetProperty(nameof(t91.Id))!, new() { Setter = (dto, val) => ((t91)dto).Id = (int)val! }),
                ])
            });
        // TestCases.OnBeforeAfterValidationTest.Request
        var _t92 = typeof(t92);
        cache.TryAdd(
            _t92,
            new()
            {
                ObjectFactory = () => new t92(),
                Properties = new(
                [
                     new(_t92.GetProperty(nameof(t92.Verb))!, new() { Setter = (dto, val) => ((t92)dto).Verb = (FastEndpoints.Http)val! }),
                     new(_t92.GetProperty(nameof(t92.Host))!, new() { Setter = (dto, val) => ((t92)dto).Host = (string?)val! }),
                ])
            });
        // TestCases.PlainTextRequestTest.Request
        var _t93 = typeof(t93);
        cache.TryAdd(
            _t93,
            new()
            {
                ObjectFactory = () => new t93(),
                Properties = new(
                [
                     new(_t93.GetProperty(nameof(t93.Id))!, new() { Setter = (dto, val) => ((t93)dto).Id = (int)val! }),
                     new(_t93.GetProperty(nameof(t93.Content))!, new() { Setter = (dto, val) => ((t93)dto).Content = (string)val! }),
                ])
            });
        // TestCases.TypedResultTest.Request
        var _t94 = typeof(t94);
        cache.TryAdd(
            _t94,
            new()
            {
                ObjectFactory = () => new t94(),
                Properties = new(
                [
                     new(_t94.GetProperty(nameof(t94.Id))!, new() { Setter = (dto, val) => ((t94)dto).Id = (int)val! }),
                ])
            });
        // TestCases.Idempotency.Request
        var _t95 = typeof(t95);
        cache.TryAdd(
            _t95,
            new()
            {
                ObjectFactory = () => new t95(),
                Properties = new(
                [
                     new(_t95.GetProperty(nameof(t95.Content))!, new() { Setter = (dto, val) => ((t95)dto).Content = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.SomeCommand
        var _t97 = typeof(t97);
        cache.TryAdd(
            _t97,
            new()
            {
                ObjectFactory = () => new t97(),
                Properties = new(
                [
                     new(_t97.GetProperty(nameof(t97.FirstName))!, new() { Setter = (dto, val) => ((t97)dto).FirstName = (string)val! }),
                     new(_t97.GetProperty(nameof(t97.LastName))!, new() { Setter = (dto, val) => ((t97)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.EchoCommand
        var _t99 = typeof(t99);
        cache.TryAdd(
            _t99,
            new()
            {
                ObjectFactory = () => new t99(),
                Properties = new(
                [
                     new(_t99.GetProperty(nameof(t99.FirstName))!, new() { Setter = (dto, val) => ((t99)dto).FirstName = (string)val! }),
                     new(_t99.GetProperty(nameof(t99.LastName))!, new() { Setter = (dto, val) => ((t99)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.VoidCommand
        var _t101 = typeof(t101);
        cache.TryAdd(
            _t101,
            new()
            {
                ObjectFactory = () => new t101(),
                Properties = new(
                [
                     new(_t101.GetProperty(nameof(t101.FirstName))!, new() { Setter = (dto, val) => ((t101)dto).FirstName = (string)val! }),
                     new(_t101.GetProperty(nameof(t101.LastName))!, new() { Setter = (dto, val) => ((t101)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandHandlerTest.GetFullName
        var _t103 = typeof(t103);
        cache.TryAdd(
            _t103,
            new()
            {
                ObjectFactory = () => new t103(),
                Properties = new(
                [
                     new(_t103.GetProperty(nameof(t103.FirstName))!, new() { Setter = (dto, val) => ((t103)dto).FirstName = (string)val! }),
                     new(_t103.GetProperty(nameof(t103.LastName))!, new() { Setter = (dto, val) => ((t103)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.JobQueueTest.JobCancelTestCommand
        var _t105 = typeof(t105);
        cache.TryAdd(
            _t105,
            new()
            {
                ObjectFactory = () => new t105(),
                Properties = new(
                [
                     new(_t105.GetProperty(nameof(t105.TrackingId))!, new() { Setter = (dto, val) => ((t105)dto).TrackingId = (System.Guid)val! }),
                     new(_t105.GetProperty(nameof(t105.Counter))!, new() { Setter = (dto, val) => ((t105)dto).Counter = (int)val! }),
                     new(_t105.GetProperty(nameof(t105.IsCancelled))!, new() { Setter = (dto, val) => ((t105)dto).IsCancelled = (bool)val! }),
                ])
            });
        // TestCases.JobQueueTest.JobProgressTestCommand
        var _t107 = typeof(t107);
        cache.TryAdd(
            _t107,
            new()
            {
                ObjectFactory = () => new t107(),
                Properties = new(
                [
                     new(_t107.GetProperty(nameof(t107.TrackingID))!, new() { Setter = (dto, val) => ((t107)dto).TrackingID = (System.Guid)val! }),
                     new(_t107.GetProperty(nameof(t107.Name))!, new() { Setter = (dto, val) => ((t107)dto).Name = (string)val! }),
                ])
            });
        // TestCases.JobQueueTest.JobTestCommand
        var _t109 = typeof(t109);
        cache.TryAdd(
            _t109,
            new()
            {
                ObjectFactory = () => new t109(),
                Properties = new(
                [
                     new(_t109.GetProperty(nameof(t109.Id))!, new() { Setter = (dto, val) => ((t109)dto).Id = (int)val! }),
                     new(_t109.GetProperty(nameof(t109.ShouldThrow))!, new() { Setter = (dto, val) => ((t109)dto).ShouldThrow = (bool)val! }),
                     new(_t109.GetProperty(nameof(t109.ThrowCount))!, new() { Setter = (dto, val) => ((t109)dto).ThrowCount = (int)val! }),
                ])
            });
        // TestCases.JobQueueTest.JobWithResultTestCommand
        var _t111 = typeof(t111);
        cache.TryAdd(
            _t111,
            new()
            {
                ObjectFactory = () => new t111(),
                Properties = new(
                [
                     new(_t111.GetProperty(nameof(t111.Id))!, new() { Setter = (dto, val) => ((t111)dto).Id = (System.Guid)val! }),
                ])
            });
        // TestCases.GlobalRoutePrefixOverrideTest.Request
        var _t112 = typeof(t112);
        cache.TryAdd(
            _t112,
            new()
            {
                ObjectFactory = () => new t112(),
                Properties = new(
                [
                     new(_t112.GetProperty(nameof(t112.Id))!, new() { Setter = (dto, val) => ((t112)dto).Id = (int)val! }),
                     new(_t112.GetProperty(nameof(t112.Content))!, new() { Setter = (dto, val) => ((t112)dto).Content = (string)val! }),
                ])
            });
        // TestCases.MapperTest.Request
        var _t113 = typeof(t113);
        cache.TryAdd(
            _t113,
            new()
            {
                ObjectFactory = () => new t113(),
                Properties = new(
                [
                     new(_t113.GetProperty(nameof(t113.FirstName))!, new() { Setter = (dto, val) => ((t113)dto).FirstName = (string)val! }),
                     new(_t113.GetProperty(nameof(t113.LastName))!, new() { Setter = (dto, val) => ((t113)dto).LastName = (string)val! }),
                     new(_t113.GetProperty(nameof(t113.Age))!, new() { Setter = (dto, val) => ((t113)dto).Age = (int)val! }),
                ])
            });
        // TestCases.STJInfiniteRecursionTest.Response
        var _t114 = typeof(t114);
        cache.TryAdd(
            _t114,
            new()
            {
                ObjectFactory = () => new t114(),
                Properties = new(
                [
                     new(_t114.GetProperty(nameof(t114.Id))!, new() { Setter = (dto, val) => ((t114)dto).Id = (int)val! }),
                     new(_t114.GetProperty(nameof(t114.Res))!, new() { Setter = (dto, val) => ((t114)dto).Res = (TestCases.STJInfiniteRecursionTest.Response?)val! }),
                ])
            });
        // TestCases.UnitTestConcurrencyTest.Request
        var _t115 = typeof(t115);
        cache.TryAdd(
            _t115,
            new()
            {
                ObjectFactory = () => new t115(),
                Properties = new(
                [
                     new(_t115.GetProperty(nameof(t115.Id))!, new() { Setter = (dto, val) => ((t115)dto).Id = (int)val! }),
                ])
            });
        // TestCases.UnitTestConcurrencyTest.Endpoint
        var _t116 = typeof(t116);
        cache.TryAdd(
            _t116,
            new()
            {
                Properties = new(
                [
                     new(_t116.GetProperty(nameof(t116.Svc))!, new() { Setter = (dto, val) => ((t116)dto).Svc = (TestCases.UnitTestConcurrencyTest.SingltonSVC)val! }),
                ])
            });
        // TestCases.GlobalGenericProcessorTest.Request
        var _t117 = typeof(t117);
        cache.TryAdd(
            _t117,
            new()
            {
                ObjectFactory = () => new t117(),
                Properties = new(
                [
                     new(_t117.GetProperty(nameof(t117.PreProcRan))!, new() { Setter = (dto, val) => ((t117)dto).PreProcRan = (bool)val! }),
                     new(_t117.GetProperty(nameof(t117.PostProcRan))!, new() { Setter = (dto, val) => ((t117)dto).PostProcRan = (bool)val! }),
                ])
            });
        // TestCases.PostProcessorTest.Request
        var _t118 = typeof(t118);
        cache.TryAdd(
            _t118,
            new()
            {
                ObjectFactory = () => new t118(),
                Properties = new(
                [
                     new(_t118.GetProperty(nameof(t118.Id))!, new() { Setter = (dto, val) => ((t118)dto).Id = (int)val! }),
                ])
            });
        // TestCases.ProcessorAttributesTest.Request
        var _t119 = typeof(t119);
        cache.TryAdd(
            _t119,
            new()
            {
                ObjectFactory = () => new t119(),
                Properties = new(
                [
                     new(_t119.GetProperty(nameof(t119.Values))!, new() { Setter = (dto, val) => ((t119)dto).Values = (System.Collections.Generic.List<string>)val! }),
                ])
            });
        // TestCases.ProcessorStateTest.Request
        var _t120 = typeof(t120);
        cache.TryAdd(
            _t120,
            new()
            {
                ObjectFactory = () => new t120(),
                Properties = new(
                [
                     new(_t120.GetProperty(nameof(t120.Id))!, new() { Setter = (dto, val) => ((t120)dto).Id = (int)val! }),
                ])
            });
        // TestCases.Routing.NonOptionalRouteParamTest.Request
        var _t121 = typeof(t121);
        cache.TryAdd(
            _t121,
            new()
            {
                ObjectFactory = () => new t121(default!),
                Properties = new(
                [
                     new(_t121.GetProperty(nameof(t121.UserId))!, new()),
                ])
            });
        // TestCases.Routing.OptionalRouteParamTest.Request
        var _t122 = typeof(t122);
        cache.TryAdd(
            _t122,
            new()
            {
                ObjectFactory = () => new t122(default!),
                Properties = new(
                [
                     new(_t122.GetProperty(nameof(t122.OfferId))!, new()),
                ])
            });
        // TestCases.AntiforgeryTest.TokenResponse
        var _t123 = typeof(t123);
        cache.TryAdd(
            _t123,
            new()
            {
                ObjectFactory = () => new t123(),
                Properties = new(
                [
                     new(_t123.GetProperty(nameof(t123.TokenName))!, new() { Setter = (dto, val) => ((t123)dto).TokenName = (string)val! }),
                     new(_t123.GetProperty(nameof(t123.Value))!, new() { Setter = (dto, val) => ((t123)dto).Value = (string?)val! }),
                ])
            });
        // TestCases.AntiforgeryTest.VerificationRequest
        var _t124 = typeof(t124);
        cache.TryAdd(
            _t124,
            new()
            {
                ObjectFactory = () => new t124(),
                Properties = new(
                [
                     new(_t124.GetProperty(nameof(t124.File))!, new() { Setter = (dto, val) => ((t124)dto).File = (Microsoft.AspNetCore.Http.IFormFile)val! }),
                     new(_t124.GetProperty(nameof(t124.TokenResponse))!, new() { Setter = (dto, val) => ((t124)dto).TokenResponse = (TestCases.AntiforgeryTest.TokenResponse)val! }),
                ])
            });
        // TestCases.MissingClaimTest.DontThrowIfMissingRequest
        var _t125 = typeof(t125);
        cache.TryAdd(
            _t125,
            new()
            {
                ObjectFactory = () => new t125(),
                Properties = new(
                [
                     new(_t125.GetProperty(nameof(t125.TestProp))!, new() { Setter = (dto, val) => ((t125)dto).TestProp = (string?)val! }),
                ])
            });
        // TestCases.MissingClaimTest.ThrowIfMissingRequest
        var _t126 = typeof(t126);
        cache.TryAdd(
            _t126,
            new()
            {
                ObjectFactory = () => new t126(),
                Properties = new(
                [
                     new(_t126.GetProperty(nameof(t126.TestProp))!, new() { Setter = (dto, val) => ((t126)dto).TestProp = (string?)val! }),
                ])
            });
        // TestCases.MissingHeaderTest.DontThrowIfMissingRequest
        var _t127 = typeof(t127);
        cache.TryAdd(
            _t127,
            new()
            {
                ObjectFactory = () => new t127(),
                Properties = new(
                [
                     new(_t127.GetProperty(nameof(t127.TenantID))!, new() { Setter = (dto, val) => ((t127)dto).TenantID = (string?)val! }),
                ])
            });
        // TestCases.MissingHeaderTest.ThrowIfMissingRequest
        var _t128 = typeof(t128);
        cache.TryAdd(
            _t128,
            new()
            {
                ObjectFactory = () => new t128(),
                Properties = new(
                [
                     new(_t128.GetProperty(nameof(t128.TenantID))!, new() { Setter = (dto, val) => ((t128)dto).TenantID = (string?)val! }),
                ])
            });
        // TestCases.RateLimitTests.Response
        var _t129 = typeof(t129);
        cache.TryAdd(
            _t129,
            new()
            {
                ObjectFactory = () => new t129(),
                Properties = new(
                [
                     new(_t129.GetProperty(nameof(t129.CustomerID))!, new() { Setter = (dto, val) => ((t129)dto).CustomerID = (int)val! }),
                     new(_t129.GetProperty(nameof(t129.OtherID))!, new() { Setter = (dto, val) => ((t129)dto).OtherID = (int?)val! }),
                ])
            });
        // FastEndpoints.Security.TokenResponse
        var _t130 = typeof(t130);
        cache.TryAdd(
            _t130,
            new()
            {
                ObjectFactory = () => new t130(),
                Properties = new(
                [
                     new(_t130.GetProperty(nameof(t130.AccessToken))!, new() { Setter = (dto, val) => ((t130)dto).AccessToken = (string)val! }),
                     new(_t130.GetProperty(nameof(t130.UserId))!, new() { Setter = (dto, val) => ((t130)dto).UserId = (string)val! }),
                     new(_t130.GetProperty(nameof(t130.RefreshToken))!, new() { Setter = (dto, val) => ((t130)dto).RefreshToken = (string)val! }),
                ])
            });
        // FastEndpoints.Security.TokenRequest
        var _t131 = typeof(t131);
        cache.TryAdd(
            _t131,
            new()
            {
                ObjectFactory = () => new t131(),
                Properties = new(
                [
                     new(_t131.GetProperty(nameof(t131.UserId))!, new() { Setter = (dto, val) => ((t131)dto).UserId = (string)val! }),
                     new(_t131.GetProperty(nameof(t131.RefreshToken))!, new() { Setter = (dto, val) => ((t131)dto).RefreshToken = (string)val! }),
                ])
            });
        // TestCases.DataAnnotationCompliant.Request
        var _t132 = typeof(t132);
        cache.TryAdd(
            _t132,
            new()
            {
                ObjectFactory = () => new t132(),
                Properties = new(
                [
                     new(_t132.GetProperty(nameof(t132.Id))!, new()),
                     new(_t132.GetProperty(nameof(t132.Name))!, new() { Setter = (dto, val) => ((t132)dto).Name = (string)val! }),
                     new(_t132.GetProperty(nameof(t132.Meta))!, new() { Setter = (dto, val) => ((t132)dto).Meta = (TestCases.DataAnnotationCompliant.NestedRequest)val! }),
                     new(_t132.GetProperty(nameof(t132.Children))!, new() { Setter = (dto, val) => ((t132)dto).Children = (System.Collections.Generic.List<TestCases.DataAnnotationCompliant.ChildRequest>)val! }),
                ])
            });
        // TestCases.DataAnnotationCompliant.NestedRequest
        var _t134 = typeof(t134);
        cache.TryAdd(
            _t134,
            new()
            {
                ObjectFactory = () => new t134(),
                Properties = new(
                [
                     new(_t134.GetProperty(nameof(t134.Gender))!, new() { Setter = (dto, val) => ((t134)dto).Gender = (string)val! }),
                     new(_t134.GetProperty(nameof(t134.Age))!, new() { Setter = (dto, val) => ((t134)dto).Age = (int)val! }),
                ])
            });
        // TestCases.DataAnnotationCompliant.ChildRequest
        var _t136 = typeof(t136);
        cache.TryAdd(
            _t136,
            new()
            {
                ObjectFactory = () => new t136(),
                Properties = new(
                [
                     new(_t136.GetProperty(nameof(t136.Name))!, new() { Setter = (dto, val) => ((t136)dto).Name = (string)val! }),
                     new(_t136.GetProperty(nameof(t136.Age))!, new() { Setter = (dto, val) => ((t136)dto).Age = (int)val! }),
                     new(_t136.GetProperty(nameof(t136.Gender))!, new() { Setter = (dto, val) => ((t136)dto).Gender = (string)val! }),
                ])
            });
        // TestCases.PreProcessorShortWhileValidatorFails.Request
        var _t137 = typeof(t137);
        cache.TryAdd(
            _t137,
            new()
            {
                ObjectFactory = () => new t137(),
                Properties = new(
                [
                     new(_t137.GetProperty(nameof(t137.Id))!, new() { Setter = (dto, val) => ((t137)dto).Id = (int)val! }),
                ])
            });
        // TestCases.PreProcessorIsRunOnValidationFailure.Request
        var _t138 = typeof(t138);
        cache.TryAdd(
            _t138,
            new()
            {
                ObjectFactory = () => new t138(),
                Properties = new(
                [
                     new(_t138.GetProperty(nameof(t138.FirstName))!, new() { Setter = (dto, val) => ((t138)dto).FirstName = (string?)val! }),
                     new(_t138.GetProperty(nameof(t138.FailureCount))!, new() { Setter = (dto, val) => ((t138)dto).FailureCount = (int)val! }),
                ])
            });
        // TestCases.ValidationErrorTest.ArrayRequest
        var _t139 = typeof(t139);
        cache.TryAdd(
            _t139,
            new()
            {
                ObjectFactory = () => new t139(),
                Properties = new(
                [
                     new(_t139.GetProperty(nameof(t139.StringArray))!, new()),
                ])
            });
        // TestCases.ValidationErrorTest.DictionaryRequest
        var _t140 = typeof(t140);
        cache.TryAdd(
            _t140,
            new()
            {
                ObjectFactory = () => new t140(),
                Properties = new(
                [
                     new(_t140.GetProperty(nameof(t140.StringDictionary))!, new()),
                ])
            });
        // TestCases.ValidationErrorTest.ListInListRequest
        var _t141 = typeof(t141);
        cache.TryAdd(
            _t141,
            new()
            {
                ObjectFactory = () => new t141(),
                Properties = new(
                [
                     new(_t141.GetProperty(nameof(t141.NumbersList))!, new()),
                ])
            });
        // TestCases.ValidationErrorTest.ListRequest
        var _t142 = typeof(t142);
        cache.TryAdd(
            _t142,
            new()
            {
                ObjectFactory = () => new t142(),
                Properties = new(
                [
                     new(_t142.GetProperty(nameof(t142.NumbersList))!, new()),
                ])
            });
        // TestCases.ValidationErrorTest.ObjectArrayRequest
        var _t143 = typeof(t143);
        cache.TryAdd(
            _t143,
            new()
            {
                ObjectFactory = () => new t143(),
                Properties = new(
                [
                     new(_t143.GetProperty(nameof(t143.ObjectArray))!, new()),
                ])
            });
        // TestCases.ValidationErrorTest.TObject
        var _t145 = typeof(t145);
        cache.TryAdd(
            _t145,
            new()
            {
                ObjectFactory = () => new t145(),
                Properties = new(
                [
                     new(_t145.GetProperty(nameof(t145.Test))!, new()),
                ])
            });
        return cache;
    }
    /// <summary>
    /// register pre-generated command handler executors from the source generator.
    /// this enables AOT compatibility by avoiding MakeGenericType at runtime.
    /// </summary>
    public static void RegisterCommandExecutors(CommandHandlerRegistry registry, IServiceProvider sp)
    {
        // TestCases.CommandBusTest.SomeCommand -> TestCases.CommandBusTest.SomeCommandHandler
        registry[typeof(TestCases.CommandBusTest.SomeCommand)] = new(typeof(TestCases.CommandBusTest.SomeCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.CommandBusTest.SomeCommand, string>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.SomeCommand, string>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.SomeCommand, string>>())
        };
        // TestCases.CommandBusTest.EchoCommand -> TestCases.CommandBusTest.EchoCommandHandler
        registry[typeof(TestCases.CommandBusTest.EchoCommand)] = new(typeof(TestCases.CommandBusTest.EchoCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.CommandBusTest.EchoCommand, TestCases.CommandBusTest.EchoCommand>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.EchoCommand, TestCases.CommandBusTest.EchoCommand>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.EchoCommand, TestCases.CommandBusTest.EchoCommand>>())
        };
        // TestCases.CommandBusTest.VoidCommand -> TestCases.CommandBusTest.VoidCommandHandler
        registry[typeof(TestCases.CommandBusTest.VoidCommand)] = new(typeof(TestCases.CommandBusTest.VoidCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.CommandBusTest.VoidCommand, FastEndpoints.Void>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.VoidCommand, FastEndpoints.Void>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.CommandBusTest.VoidCommand, FastEndpoints.Void>>())
        };
        // TestCases.CommandHandlerTest.GetFullName -> TestCases.CommandHandlerTest.MakeFullName
        registry[typeof(TestCases.CommandHandlerTest.GetFullName)] = new(typeof(TestCases.CommandHandlerTest.MakeFullName))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.CommandHandlerTest.GetFullName, string>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.CommandHandlerTest.GetFullName, string>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.CommandHandlerTest.GetFullName, string>>())
        };
        // TestCases.JobQueueTest.JobCancelTestCommand -> TestCases.JobQueueTest.JobCancelTestCommandHandler
        registry[typeof(TestCases.JobQueueTest.JobCancelTestCommand)] = new(typeof(TestCases.JobQueueTest.JobCancelTestCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.JobQueueTest.JobCancelTestCommand, FastEndpoints.Void>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobCancelTestCommand, FastEndpoints.Void>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobCancelTestCommand, FastEndpoints.Void>>())
        };
        // TestCases.JobQueueTest.JobProgressTestCommand -> TestCases.JobQueueTest.JobProgressTestCmdHandler
        registry[typeof(TestCases.JobQueueTest.JobProgressTestCommand)] = new(typeof(TestCases.JobQueueTest.JobProgressTestCmdHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.JobQueueTest.JobProgressTestCommand, FastEndpoints.JobResult<string>>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobProgressTestCommand, FastEndpoints.JobResult<string>>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobProgressTestCommand, FastEndpoints.JobResult<string>>>())
        };
        // TestCases.JobQueueTest.JobTestCommand -> TestCases.JobQueueTest.JobTestCommandHandler
        registry[typeof(TestCases.JobQueueTest.JobTestCommand)] = new(typeof(TestCases.JobQueueTest.JobTestCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.JobQueueTest.JobTestCommand, FastEndpoints.Void>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobTestCommand, FastEndpoints.Void>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobTestCommand, FastEndpoints.Void>>())
        };
        // TestCases.JobQueueTest.JobWithResultTestCommand -> TestCases.JobQueueTest.JobWithResultTestCommandHandler
        registry[typeof(TestCases.JobQueueTest.JobWithResultTestCommand)] = new(typeof(TestCases.JobQueueTest.JobWithResultTestCommandHandler))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.JobQueueTest.JobWithResultTestCommand, System.Guid>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobWithResultTestCommand, System.Guid>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.JobQueueTest.JobWithResultTestCommand, System.Guid>>())
        };
    }
}