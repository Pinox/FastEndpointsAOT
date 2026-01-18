namespace TestCases.ValidationErrorTest;

public class ListInListRequest
{
    public List<List<int>> NumbersList { get; init; }
}

public class ListInListValidationErrorTestEndpoint : Endpoint<ListInListRequest>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_list_in_list_validation_error_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(ListInListRequest req, CancellationToken c)
    {

        for (int i = 0; i < req.NumbersList.Count; i++)
        {
            for (int j = 0; j < req.NumbersList[i].Count; j++)
            {
                AddError(r => r.NumbersList[i][j], "Invalid");
            }
        }
        return Task.CompletedTask;
    }
}