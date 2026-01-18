namespace Security.Policies.AdminOnly;

// GET /policies/admin -> requires AdminPolicy (only username 'admin')
public sealed class AdminPolicyVerify : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get(AppRoutes.policies_AdminPolicyVerify);
        AuthSchemes("Cookies", "Bearer");
        Policies(PolicyNames.AdminRole);
        Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
        Summary(s =>
        {
            s.Summary = nameof(AdminPolicyVerify);
            s.Description = "Accessible only by the admin user (username 'admin').";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
        => await Send.OkAsync("admin-policy-ok");
}