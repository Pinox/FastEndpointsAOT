using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        // Generate the extension methods
        var extensionsSource = GenerateExtensions(validEndpoints);
        
        // Also generate a debug file to see what we found
        var debugInfo = new StringBuilder();
        debugInfo.AppendLine("// Found endpoints:");
        foreach (var ep in validEndpoints)
        {
            debugInfo.AppendLine($"// - {ep.EndpointNamespace}.{ep.EndpointName}: {ep.HttpMethod} {ep.Route}");
            debugInfo.AppendLine($"//   Request: {ep.RequestTypeFullName}, Response: {ep.ResponseTypeFullName}");
        }
        context.AddSource("ApiRequestDebug.g.cs", debugInfo.ToString());
        
        // Output to the Web project (standard source generator output)
        context.AddSource("ApiRequestExtensions.g.cs", extensionsSource);
    }

    private static string GenerateExtensions(List<EndpointInfo> endpoints)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("namespace Shared");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// AOT-compatible API request extensions using source-generated JSON serialization.");
        sb.AppendLine("    /// Each method accepts a JsonSerializerContext for AOT-safe serialization.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static class GeneratedApiRequestExtensions");
        sb.AppendLine("    {");
        
        // Track used method names to avoid duplicates
        var usedMethodNames = new HashSet<string>();

        foreach (var endpoint in endpoints)
        {
            // Use fully qualified type names with global:: to avoid any ambiguity
            var requestTypeFullName = $"global::{endpoint.RequestTypeFullName}";
            var responseTypeFullName = endpoint.ResponseTypeFullName;
            
            // Handle special types like object? - can't use global:: on object
            if (responseTypeFullName == "object?" || responseTypeFullName == "object")
            {
                responseTypeFullName = "object";
            }
            else
            {
                // Strip trailing ? for now, we'll add it back
                responseTypeFullName = responseTypeFullName.TrimEnd('?');
                responseTypeFullName = $"global::{responseTypeFullName}";
            }

            // Create unique method name based on full namespace + endpoint class name
            var baseMethodName = endpoint.HttpMethod switch
            {
                "Get" => "Get",
                "Put" => "Put",
                "Delete" => "Delete",
                "Patch" => "Patch",
                _ => "Post"
            };
            
            // Create a safe method name from the endpoint namespace and name
            var safeNamespace = endpoint.EndpointNamespace.Replace(".", "_").Replace(" ", "_").Replace("<", "").Replace(">", "");
            var uniqueMethodName = $"{baseMethodName}_{safeNamespace}_{endpoint.EndpointName}";
            
            // Fallback if still duplicate
            var counter = 1;
            var finalMethodName = uniqueMethodName;
            while (usedMethodNames.Contains(finalMethodName))
            {
                finalMethodName = $"{uniqueMethodName}_{counter++}";
            }
            usedMethodNames.Add(finalMethodName);

            var hasBody = endpoint.HttpMethod is "Post" or "Put" or "Patch";

            sb.AppendLine();
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// Sends a {endpoint.HttpMethod.ToUpper()} request to {endpoint.Route}");
            sb.AppendLine($"        /// </summary>");
            sb.AppendLine($"        /// <param name=\"client\">The HTTP client.</param>");
            sb.AppendLine($"        /// <param name=\"request\">The request DTO.</param>");
            sb.AppendLine($"        /// <param name=\"requestTypeInfo\">The JSON type info for the request type (for AOT).</param>");
            sb.AppendLine($"        /// <param name=\"responseTypeInfo\">The JSON type info for the response type (for AOT).</param>");
            sb.AppendLine($"        /// <param name=\"cancellationToken\">Cancellation token.</param>");
            sb.AppendLine($"        public static async global::System.Threading.Tasks.Task<(global::System.Net.Http.HttpResponseMessage Response, {responseTypeFullName}? Result)> {finalMethodName}(");
            sb.AppendLine($"            this global::System.Net.Http.HttpClient client,");
            sb.AppendLine($"            {requestTypeFullName} request,");
            sb.AppendLine($"            global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<{requestTypeFullName}> requestTypeInfo,");
            sb.AppendLine($"            global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<{responseTypeFullName}> responseTypeInfo,");
            sb.AppendLine($"            global::System.Threading.CancellationToken cancellationToken = default)");
            sb.AppendLine("        {");

            if (hasBody)
            {
                sb.AppendLine($"            var json = global::System.Text.Json.JsonSerializer.Serialize(request, requestTypeInfo);");
                sb.AppendLine($"            var content = new global::System.Net.Http.StringContent(json, global::System.Text.Encoding.UTF8, \"application/json\");");
                sb.AppendLine($"            var response = await client.PostAsync(\"{endpoint.Route}\", content, cancellationToken);");
            }
            else
            {
                sb.AppendLine($"            var response = await client.GetAsync(\"{endpoint.Route}\", cancellationToken);");
            }

            sb.AppendLine();
            sb.AppendLine($"            {responseTypeFullName}? result = default;");
            sb.AppendLine("            if (response.IsSuccessStatusCode)");
            sb.AppendLine("            {");
            sb.AppendLine($"                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);");
            sb.AppendLine($"                result = global::System.Text.Json.JsonSerializer.Deserialize(responseJson, responseTypeInfo);");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return (response, result);");
            sb.AppendLine("        }");
        }
        
        sb.AppendLine("    }");
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
