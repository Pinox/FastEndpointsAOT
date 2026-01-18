namespace Security.Policies.UserOnly;

// GET /policies/user -> requires UserPolicy (any authenticated user)
public sealed class UserPolicyVerify : EndpointWithoutRequest<string>
{
 public override void Configure()
 {
 Get(AppRoutes.policies_UserPolicyVerify);
 Policies(PolicyNames.User);
 Description(b => b.WithTags($"Heading:{ApiHeadings.Security}"));
 Summary(s =>
 {
 s.Summary = nameof(UserPolicyVerify);
 s.Description = "Accessible by any authenticated user (UserPolicy).";
 });
 }

 public override async Task HandleAsync(CancellationToken ct)
 => await Send.OkAsync("user-policy-ok");
}
