namespace TestCases.ValidationErrorTest;

public class ListRequest
{
    public List<int> NumbersList { get; init; }
}

public class ListValidationErrorTestEndpoint : Endpoint<ListRequest>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_list_validation_error_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(ListRequest req, CancellationToken c)
    {

        for (int i = 0; i < req.NumbersList.Count; i++)
        {
            AddError(r => r.NumbersList[i], "Invalid");
        }
        return Task.CompletedTask;
    }
}