// ReSharper disable InconsistentNaming

namespace TestCases.Swagger.ReleaseVersioning;

sealed class EndpointA : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AppRoutes.release_versioning_endpoint_a);
        Tags("release_versioning");
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}

sealed class EndpointA_V1 : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AppRoutes.release_versioning_endpoint_a);
        Tags("release_versioning");
        Version(1).DeprecateAt(2);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}

sealed class EndpointA_V2 : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AppRoutes.release_versioning_endpoint_a);
        Tags("release_versioning");
        Version(2)
            .StartingRelease(3)
            .DeprecateAt(4);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}

sealed class EndpointB : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AppRoutes.release_versioning_endpoint_b);
        Tags("release_versioning");
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}

sealed class EndpointB_V1 : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AppRoutes.release_versioning_endpoint_b);
        Tags("release_versioning");
        Version(1).StartingRelease(2);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}

sealed class EndpointB_V1_Delete : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete(AppRoutes.release_versioning_endpoint_b);
        Tags("release_versioning");
        Version(1).StartingRelease(2);
        Description(b => b.WithTags("Hide"));
    }

    public override Task HandleAsync(CancellationToken c)
        => Task.CompletedTask;
}