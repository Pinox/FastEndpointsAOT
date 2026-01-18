using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiRequestGenerator;

[Generator]
public class ApiRequestSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all classes that inherit from Endpoint<TRequest, TResponse>
        var endpointDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsEndpointClass(node),
                transform: static (ctx, _) => GetEndpointInfo(ctx))
            .Where(static info => info is not null);

        // Combine with compilation and additional text for config
        var compilationAndEndpoints = context.CompilationProvider
            .Combine(endpointDeclarations.Collect())
            .Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(compilationAndEndpoints, Execute);
    }

    private static bool IsEndpointClass(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDecl &&
               classDecl.BaseList?.Types.Count > 0;
    }

    private static EndpointInfo? GetEndpointInfo(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl);
        
        if (symbol is null) return null;

        var baseType = symbol.BaseType;
        if (baseType is null) return null;

        // Check if it inherits from Endpoint<TRequest, TResponse>
        if (!IsEndpointBaseType(baseType, out var requestType, out var responseType))
            return null;

        // Get the route from Configure() method if possible
        var route = ExtractRoute(classDecl, context.SemanticModel);
        var httpMethod = ExtractHttpMethod(classDecl, context.SemanticModel);

        return new EndpointInfo
        {
            EndpointName = symbol.Name,
            EndpointNamespace = symbol.ContainingNamespace.ToDisplayString(),
            RequestTypeName = requestType!.Name,
            RequestTypeNamespace = requestType.ContainingNamespace.ToDisplayString(),
            RequestTypeFullName = requestType.ToDisplayString(),
            ResponseTypeName = responseType!.Name,
            ResponseTypeNamespace = responseType.ContainingNamespace.ToDisplayString(),
            ResponseTypeFullName = responseType.ToDisplayString(),
            Route = route,
            HttpMethod = httpMethod
        };
    }

    private static bool IsEndpointBaseType(INamedTypeSymbol baseType, out INamedTypeSymbol? requestType, out INamedTypeSymbol? responseType)
    {
        requestType = null;
        responseType = null;

        // Walk up the inheritance chain
        var current = baseType;
        while (current != null)
        {
            var typeName = current.OriginalDefinition.ToDisplayString();
            
            // Check for Endpoint<TRequest, TResponse>
            if (typeName.StartsWith("FastEndpoints.Endpoint<") && current.TypeArguments.Length == 2)
            {
                requestType = current.TypeArguments[0] as INamedTypeSymbol;
                responseType = current.TypeArguments[1] as INamedTypeSymbol;
                return requestType != null && responseType != null;
            }

            // Check for Ep<TRequest, TResponse> (shorthand)
            if (typeName.StartsWith("FastEndpoints.Ep<") && current.TypeArguments.Length == 2)
            {
                requestType = current.TypeArguments[0] as INamedTypeSymbol;
                responseType = current.TypeArguments[1] as INamedTypeSymbol;
                return requestType != null && responseType != null;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static string? ExtractRoute(ClassDeclarationSyntax classDecl, SemanticModel semanticModel)
    {
        // Look for Configure() method and extract Route() call
        var configureMethod = classDecl.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.Text == "Configure");

        if (configureMethod?.Body is null) return null;

        // Find invocations like Post("/route"), Get("/route"), etc.
        var invocations = configureMethod.Body.DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocations)
        {
            if (invocation.Expression is IdentifierNameSyntax identifier)
            {
                var name = identifier.Identifier.Text;
                if (name is "Post" or "Get" or "Put" or "Delete" or "Patch" or "Routes")
                {
                    var args = invocation.ArgumentList.Arguments;
                    if (args.Count > 0 && args[0].Expression is LiteralExpressionSyntax literal)
                    {
                        return literal.Token.ValueText;
                    }
                }
            }
        }

        return null;
    }

    private static string ExtractHttpMethod(ClassDeclarationSyntax classDecl, SemanticModel semanticModel)
    {
        var configureMethod = classDecl.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.Text == "Configure");

        if (configureMethod?.Body is null) return "Post";

        var invocations = configureMethod.Body.DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocations)
        {
            if (invocation.Expression is IdentifierNameSyntax identifier)
            {
                var name = identifier.Identifier.Text;
                if (name is "Post") return "Post";
                if (name is "Get") return "Get";
                if (name is "Put") return "Put";
                if (name is "Delete") return "Delete";
                if (name is "Patch") return "Patch";
            }
        }

        return "Post";
    }

    private static void Execute(
        SourceProductionContext context,
        ((Compilation Compilation, ImmutableArray<EndpointInfo?> Endpoints), Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider Options) input)
    {
        var (compilationAndEndpoints, optionsProvider) = input;
        var (compilation, endpoints) = compilationAndEndpoints;

        var validEndpoints = endpoints
            .Where(e => e is not null && e.Route is not null)
            .Cast<EndpointInfo>()
            .ToList();

        if (validEndpoints.Count == 0) return;

        // Try to get the shared project path from build property
        optionsProvider.GlobalOptions.TryGetValue("build_property.SharedProjectPath", out var sharedProjectPath);

        // Generate the extension methods
        var extensionsSource = GenerateExtensions(validEndpoints);
        
        // Output to the Web project (standard source generator output)
        context.AddSource("ApiRequestExtensions.g.cs", extensionsSource);

        // If SharedProjectPath is specified, also write to that location
        if (!string.IsNullOrEmpty(sharedProjectPath))
        {
            try
            {
                var generatedDir = Path.Combine(sharedProjectPath, "Generated");
                Directory.CreateDirectory(generatedDir);
                
                var filePath = Path.Combine(generatedDir, "ApiRequestExtensions.g.cs");
                File.WriteAllText(filePath, extensionsSource);
            }
            catch
            {
                // Ignore file write errors - the in-memory source is still available
            }
        }
    }

    private static string GenerateExtensions(List<EndpointInfo> endpoints)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System.Net.Http;");
        sb.AppendLine("using System.Net.Http.Json;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();

        // Group by request namespace to organize
        var byNamespace = endpoints.GroupBy(e => e.RequestTypeNamespace);

        sb.AppendLine("namespace Shared;");
        sb.AppendLine();
        sb.AppendLine("public static class GeneratedApiRequestExtensions");
        sb.AppendLine("{");

        foreach (var endpoint in endpoints)
        {
            var methodName = endpoint.HttpMethod switch
            {
                "Get" => "GetAsJsonAsync",
                "Put" => "PutAsJsonAsync",
                "Delete" => "DeleteAsJsonAsync",
                "Patch" => "PatchAsJsonAsync",
                _ => "PostAsJsonAsync"
            };

            var httpClientMethod = endpoint.HttpMethod switch
            {
                "Get" => "GetAsync",
                "Put" => "PutAsync",
                "Delete" => "DeleteAsync",
                "Patch" => "PatchAsync",
                _ => "PostAsync"
            };

            var hasBody = endpoint.HttpMethod is "Post" or "Put" or "Patch";

            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// Sends a {endpoint.HttpMethod.ToUpper()} request to {endpoint.Route}");
            sb.AppendLine($"    /// Generated from endpoint: {endpoint.EndpointNamespace}.{endpoint.EndpointName}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public static async Task<(HttpResponseMessage Response, {endpoint.ResponseTypeFullName}? Result)> {methodName}(");
            sb.AppendLine($"        this HttpClient client,");
            sb.AppendLine($"        {endpoint.RequestTypeFullName} request,");
            sb.AppendLine($"        CancellationToken cancellationToken = default)");
            sb.AppendLine($"    {{");

            if (hasBody)
            {
                sb.AppendLine($"        var content = JsonContent.Create(request, SharedJsonContext.Default.{GetJsonPropertyName(endpoint.RequestTypeName)});");
                sb.AppendLine($"        var response = await client.{httpClientMethod}(\"{endpoint.Route}\", content, cancellationToken);");
            }
            else
            {
                sb.AppendLine($"        var response = await client.{httpClientMethod}(\"{endpoint.Route}\", cancellationToken);");
            }

            sb.AppendLine();
            sb.AppendLine($"        {endpoint.ResponseTypeFullName}? result = default;");
            sb.AppendLine($"        if (response.IsSuccessStatusCode)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            result = await response.Content.ReadFromJsonAsync(SharedJsonContext.Default.{GetJsonPropertyName(endpoint.ResponseTypeName)}, cancellationToken);");
            sb.AppendLine($"        }}");
            sb.AppendLine();
            sb.AppendLine($"        return (response, result);");
            sb.AppendLine($"    }}");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GetJsonPropertyName(string typeName)
    {
        // Remove generic markers and namespace
        var name = typeName.Split('.').Last().Replace("<", "").Replace(">", "").Replace(",", "");
        return name;
    }
}

internal class EndpointInfo
{
    public string EndpointName { get; set; } = "";
    public string EndpointNamespace { get; set; } = "";
    public string RequestTypeName { get; set; } = "";
    public string RequestTypeNamespace { get; set; } = "";
    public string RequestTypeFullName { get; set; } = "";
    public string ResponseTypeName { get; set; } = "";
    public string ResponseTypeNamespace { get; set; } = "";
    public string ResponseTypeFullName { get; set; } = "";
    public string? Route { get; set; }
    public string HttpMethod { get; set; } = "Post";
}
