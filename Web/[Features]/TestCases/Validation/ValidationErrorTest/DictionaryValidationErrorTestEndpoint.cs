namespace TestCases.ValidationErrorTest;

public class DictionaryRequest
{
    public Dictionary<string, string> StringDictionary { get; init; }
}

public class DictionaryValidationErrorTestEndpoint : Endpoint<DictionaryRequest>
{
    public override void Configure()
    {
        Post(AppRoutes.testcases_dictionary_validation_error_test);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(DictionaryRequest req, CancellationToken c)
    {

        foreach (var (key, val) in req.StringDictionary)
        {
            AddError(r => r.StringDictionary[key], "Invalid");
        }
        
        return Task.CompletedTask;
    }
}