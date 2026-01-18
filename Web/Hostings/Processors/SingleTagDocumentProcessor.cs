using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Web.Hostings.Processors;

public sealed class SingleTagDocumentProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        context.Document.Tags = ApiHeadings.AllNames.Select(n => new OpenApiTag { Name = n }).ToList();
    }
}
