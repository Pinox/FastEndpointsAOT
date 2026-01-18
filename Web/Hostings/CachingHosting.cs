namespace Web.Hostings;

public static class CachingHosting
{
    public static IApplicationBuilder UseOutputCachePipeline(this IApplicationBuilder app)
        => app.UseOutputCache();
}
