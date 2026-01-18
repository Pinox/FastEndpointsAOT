namespace TestCases.JsonArrayBindingForIEnumerableProps;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_json_array_binding_for_ienumerable_props);
        AllowAnonymous();
    }

    public override Task HandleAsync(Request r, CancellationToken c)
    {
        Response.Doubles = r.Doubles;
        Response.Dates = r.Dates;
        Response.Guids = r.Guids;
        Response.Ints = r.Ints;
        Response.Steven = r.Steven;
        Response.Dict = r.Dict;
        return Task.CompletedTask;
    }
}