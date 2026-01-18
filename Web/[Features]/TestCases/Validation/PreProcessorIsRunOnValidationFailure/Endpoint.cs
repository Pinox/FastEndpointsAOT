namespace TestCases.PreProcessorIsRunOnValidationFailure;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Routes(AppRoutes.testcases_pre_processor_is_run_on_validation_failure);
        PreProcessors(new MyPreProcessor());
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(Request r, CancellationToken c)
    {
        //a validaiton failure will occur but pre processor should run
        return Send.OkAsync(Response);
    }
}