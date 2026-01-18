namespace TestCases.ValidationErrorTest;

public class ArrayRequest
{
    public string[] StringArray { get; init; } = [];
}

public class ArrayValidationErrorTestEndpoint : Endpoint<ArrayRequest>
{
    public override void Configure()
    {
        Post("testcases/array-validation-error-test");
        AllowAnonymous();
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

public class DictionaryRequest
{
    public Dictionary<string, string> StringDictionary { get; init; } = new();
}

public class DictionaryValidationErrorTestEndpoint : Endpoint<DictionaryRequest>
{
    public override void Configure()
    {
        Post("testcases/dictionary-validation-error-test");
        AllowAnonymous();
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

public class ListRequest
{
    public List<int> NumbersList { get; init; } = [];
}

public class ListValidationErrorTestEndpoint : Endpoint<ListRequest>
{
    public override void Configure()
    {
        Post("testcases/list-validation-error-test");
        AllowAnonymous();
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

public class ListInListRequest
{
    public List<List<int>> NumbersList { get; init; } = [];
}

public class ListInListValidationErrorTestEndpoint : Endpoint<ListInListRequest>
{
    public override void Configure()
    {
        Post("testcases/list-in-list-validation-error-test");
        AllowAnonymous();
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

public class TObject
{
    public string Test { get; init; } = "";
}

public class ObjectArrayRequest
{
    public TObject[] ObjectArray { get; init; } = [];
}

public class ObjectArrayValidationErrorTestEndpoint : Endpoint<ObjectArrayRequest>
{
    public override void Configure()
    {
        Post("testcases/object-array-validation-error-test");
        AllowAnonymous();
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
