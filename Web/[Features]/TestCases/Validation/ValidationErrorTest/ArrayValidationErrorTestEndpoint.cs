namespace TestCases.ValidationErrorTest;

public class ArrayRequest
{
    public string[] StringArray { get; init; }
}

public class ArrayValidationErrorTestEndpoint : Endpoint<ArrayRequest>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_array_validation_error_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(ArrayRequest req, CancellationToken c)
    {
        for (int i = 0; i < req.StringArray.Length; i++)
        {
            AddError(r => r.StringArray[i], "Invalid");
        }
        return Task.CompletedTask;
    }
}