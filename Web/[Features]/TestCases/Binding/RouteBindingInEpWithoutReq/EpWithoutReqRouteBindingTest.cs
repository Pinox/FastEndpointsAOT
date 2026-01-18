namespace TestCases.RouteBindingInEpWithoutReq;

public class EpWithoutReqRouteBindingTest : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get(AppRoutes.testcases_ep_without_req_route_binding_test);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
        => await Send.OkAsync(
            new()
            {
                CustomerID = Route<int>("customerId"),
                OtherID = Route<int>("otherId")
            });
}

public class Response
{
    public int CustomerID { get; set; }

    /// <summary>
    /// optional other id
    /// </summary>
    public int? OtherID { get; set; }
}