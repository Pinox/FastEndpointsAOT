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
using t30 = TestCases.CommandHandlerTest.GetFullName;
using t32 = TestCases.CommandBusTest.SomeCommand;
using t34 = TestCases.CommandBusTest.EchoCommand;
using t36 = TestCases.CommandBusTest.VoidCommand;

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
        // TestCases.CommandHandlerTest.GetFullName
        var _t30 = typeof(t30);
        cache.TryAdd(
            _t30,
            new()
            {
                ObjectFactory = () => new t30(),
                Properties = new(
                [
                     new(_t30.GetProperty(nameof(t30.FirstName))!, new() { Setter = (dto, val) => ((t30)dto).FirstName = (string)val! }),
                     new(_t30.GetProperty(nameof(t30.LastName))!, new() { Setter = (dto, val) => ((t30)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.SomeCommand
        var _t32 = typeof(t32);
        cache.TryAdd(
            _t32,
            new()
            {
                ObjectFactory = () => new t32(),
                Properties = new(
                [
                     new(_t32.GetProperty(nameof(t32.FirstName))!, new() { Setter = (dto, val) => ((t32)dto).FirstName = (string)val! }),
                     new(_t32.GetProperty(nameof(t32.LastName))!, new() { Setter = (dto, val) => ((t32)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.EchoCommand
        var _t34 = typeof(t34);
        cache.TryAdd(
            _t34,
            new()
            {
                ObjectFactory = () => new t34(),
                Properties = new(
                [
                     new(_t34.GetProperty(nameof(t34.FirstName))!, new() { Setter = (dto, val) => ((t34)dto).FirstName = (string)val! }),
                     new(_t34.GetProperty(nameof(t34.LastName))!, new() { Setter = (dto, val) => ((t34)dto).LastName = (string)val! }),
                ])
            });
        // TestCases.CommandBusTest.VoidCommand
        var _t36 = typeof(t36);
        cache.TryAdd(
            _t36,
            new()
            {
                ObjectFactory = () => new t36(),
                Properties = new(
                [
                     new(_t36.GetProperty(nameof(t36.FirstName))!, new() { Setter = (dto, val) => ((t36)dto).FirstName = (string)val! }),
                     new(_t36.GetProperty(nameof(t36.LastName))!, new() { Setter = (dto, val) => ((t36)dto).LastName = (string)val! }),
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
        // TestCases.CommandHandlerTest.GetFullName -> TestCases.CommandHandlerTest.MakeFullName
        registry[typeof(TestCases.CommandHandlerTest.GetFullName)] = new(typeof(TestCases.CommandHandlerTest.MakeFullName))
        {
            HandlerExecutor = new CommandHandlerExecutor<TestCases.CommandHandlerTest.GetFullName, string>(
                sp.GetService<System.Collections.Generic.IEnumerable<FastEndpoints.ICommandMiddleware<TestCases.CommandHandlerTest.GetFullName, string>>>()
                ?? System.Array.Empty<FastEndpoints.ICommandMiddleware<TestCases.CommandHandlerTest.GetFullName, string>>())
        };
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
    }
}