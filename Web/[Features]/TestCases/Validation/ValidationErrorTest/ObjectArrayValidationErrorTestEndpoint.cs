namespace TestCases.ValidationErrorTest;

public class ObjectArrayRequest
{
    public TObject[] ObjectArray { get; init; }
}

public class TObject
{
    public string Test { get; init; }
}

public class ObjectArrayValidationErrorTestEndpoint : Endpoint<ObjectArrayRequest>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_object_array_validation_error_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(ObjectArrayRequest req, CancellationToken c)
    {
        for (int i = 0; i < req.ObjectArray.Length; i++)
        {
            AddError(r => r.ObjectArray[i].Test, "Invalid");
        }
        return Task.CompletedTask;
    }
}