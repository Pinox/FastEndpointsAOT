using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Text.RegularExpressions;
using System.Linq;

namespace Web.Hostings.Processors;

public sealed class FriendlyNameOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var op = context.OperationDescription.Operation;

        // try to get the bare route (without prefix/version) from the internal FastEndpoints tag: |METHOD:bareRoute|ver|startRel|dep|
        var internalTag = op.Tags?.FirstOrDefault(t => t.StartsWith("|"));
        string? bareRoute = null;
        if (!string.IsNullOrWhiteSpace(internalTag))
        {
            var segs = internalTag.Split('|');
            if (segs.Length > 2)
            {
                var methodAndRoute = segs[1]; // METHOD:bareRoute
                var idx = methodAndRoute.IndexOf(':');
                if (idx > -1 && idx + 1 < methodAndRoute.Length)
                    bareRoute = methodAndRoute[(idx + 1)..];
            }
        }

        // fallback to the full path if the internal tag is missing
        var rawRoute = string.IsNullOrWhiteSpace(bareRoute)
                            ? (context.OperationDescription.Path ?? string.Empty).Split('?', 2)[0]
                            : bareRoute;

        // normalize: remove any leading slash
        var normalized = rawRoute.TrimStart('/');

        // Always show the route path (bare if possible) as the operation summary for Scalar
        op.Summary = normalized;

        // Keep a stable operation id derived from route & method if one isn't already provided
        var method = context.OperationDescription.Method.ToString();
        op.OperationId ??= $"{normalized}_{method}";
        return true;
    }
}
