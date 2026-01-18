using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

internal sealed class MyResolver : IJsonTypeInfoResolver
{
    private readonly IJsonTypeInfoResolver _fallback;

    [ThreadStatic]
    private static int _callDepth;

    public MyResolver(IJsonTypeInfoResolver fallback)
        => _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (options is null) throw new ArgumentNullException(nameof(options));

        // Prevent runaway recursion and produce actionable diagnostics instead of crashing the process.
        if (_callDepth++ > 64)
        {
            _callDepth--; // balance before throwing
            throw new InvalidOperationException(
                $"MyResolver recursion detected while resolving '{type.FullName}'. " +
                $"Fallback: '{_fallback.GetType().FullName}'. " +
                $"Options.TypeInfoResolver: '{options.TypeInfoResolver?.GetType().FullName ?? "<null>"}'. " +
                $"ChainCount: {options.TypeInfoResolverChain?.Count ?? 0}.");
        }

        try
        {
            // Custom handling can be added here; return a JsonTypeInfo if this resolver handles the type.
            // Otherwise delegate to the known-safe fallback resolver snapshot.
            return _fallback.GetTypeInfo(type, options);
        }
        catch (Exception ex) when (ex is not StackOverflowException)
        {
            // Wrap with context-rich error to aid debugging and telemetry.
            throw new InvalidOperationException(
                $"MyResolver failed resolving '{type.FullName}'. " +
                $"Fallback: '{_fallback.GetType().FullName}'. " +
                $"Options.TypeInfoResolver: '{options.TypeInfoResolver?.GetType().FullName ?? "<null>"}'. " +
                $"ChainCount: {options.TypeInfoResolverChain?.Count ?? 0}.", ex);
        }
        finally
        {
            _callDepth--;
        }
    }

    // Configure options in a deterministic, AOT-friendly and cycle-free way.
    public static void Configure(JsonSerializerOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));

        // Build a stable inner resolver that prefers source-generated metadata and falls back to default.
        var inner = JsonTypeInfoResolver.Combine(Web.Serialization.AppJsonContext.Default);

        // Avoid mixing chain and single-resolver semantics to prevent accidental cycles.
        options.TypeInfoResolverChain.Clear();
        options.TypeInfoResolver = new MyResolver(inner);
    }
}