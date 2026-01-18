using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Web.Hostings.Processors;

public sealed class SingleTagOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var opDesc = context.OperationDescription;
        var op = opDesc.Operation;

        var internalTags = op.Tags?.Where(t => t.StartsWith('|')).ToList() ?? new List<string>();

        // Extract reserved heading & subheading tags if present
        string? headingName = null;
        string? subheading = null;

        if (op.Tags is not null && op.Tags.Count > 0)
        {
            var heading = op.Tags.FirstOrDefault(t => t.StartsWith("Heading:", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(heading))
                headingName = heading.Substring("Heading:".Length).Trim();

            var sub = op.Tags.FirstOrDefault(t => t.StartsWith("Subheading:", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sub))
                subheading = sub.Substring("Subheading:".Length).Trim();
        }

        // Fallback subheading to the bare route from the internal FastEndpoints tag, else to the full path
        if (string.IsNullOrWhiteSpace(subheading))
        {
            // internal pipe tag: |METHOD:bareRoute|ver|startRel|dep|
            var internalTag = internalTags.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(internalTag))
            {
                var segs = internalTag.Split('|');
                if (segs.Length > 2)
                {
                    var methodAndRoute = segs[1]; // METHOD:bareRoute
                    var idx = methodAndRoute.IndexOf(':');
                    if (idx > -1 && idx + 1 < methodAndRoute.Length)
                    {
                        var bareRoute = methodAndRoute[(idx + 1)..];
                        subheading = bareRoute;
                    }
                }
            }

            subheading ??= (opDesc.Path ?? string.Empty).Split('?', 2)[0];
        }

        // normalize: remove any leading slash
        subheading = subheading?.TrimStart('/');

        // Replace visible tags with only the human-facing heading (if provided)
        if (!string.IsNullOrWhiteSpace(headingName))
            op.Tags = new List<string> { headingName! };
        else
            op.Tags = new List<string>(); // no default; force explicit headings per endpoint

        // keep internal pipe tag for the FE DocumentProcessor (it will be removed later at document stage)
        foreach (var t in internalTags)
            op.Tags.Add(t);

        // Attach subheading as an OpenAPI extension for UI/clients that can use it
        (op.ExtensionData ??= new Dictionary<string, object?>())["x-subheading"] = subheading;

        return true;
    }
}
