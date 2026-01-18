using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Shared.Contracts.Inventory;
using Web.Auth;

namespace Domain.Inventory.List.Recent;

public class Endpoint : EndpointWithoutRequest<ListRecentInventoryResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes(AppRoutes.inventory_list_recent);
        Policies(PolicyNames.AdminOnly);
        Roles(Role.Admin, Role.Staff); // Use consistent role constants
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // Keep attribute routing; only add docs Description
        Description(b => b.WithTags($"Heading:{ApiHeadings.Domain}"));
    }

    /// <summary>
    /// Handles the retrieval of recent inventory items for a specified category.
    /// </summary>
    /// <remarks>
    /// This endpoint is protected and requires the requesting user to have either the Admin or TestRole role.
    /// Additionally, the AdminOnly policy must be satisfied.
    /// </remarks>
    /// <param name="t">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response with the recent inventory items.</returns>
    public override Task HandleAsync(CancellationToken t)
    {
        Response.Category = HttpContext.GetRouteValue("CategoryID")?.ToString();

        return Send.OkAsync(Response);
    }
}