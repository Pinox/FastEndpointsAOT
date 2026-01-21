using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FastEndpoints.Generator;

/// <summary>
/// Source generator that discovers FastEndpoints types (endpoints, validators, event handlers, command handlers)
/// at compile time and generates a static list with [DynamicDependency] attributes for AOT compatibility.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DiscoveredTypesGenerator : IIncrementalGenerator
{
    #region Constants

    /// <summary>
    /// Attribute that marks types to be excluded from auto-registration.
    /// </summary>
    private const string DontRegisterAttribute = "DontRegisterAttribute";

    /// <summary>
    /// Interfaces that qualify a type for discovery.
    /// Types implementing any of these interfaces will be included in the generated list.
    /// </summary>
    private static readonly ImmutableHashSet<string> DiscoverableInterfaces = ImmutableHashSet.Create(
        "FastEndpoints.IEndpoint",
        "FastEndpoints.IEventHandler",
        "FastEndpoints.ICommandHandler",
        "FastEndpoints.ISummary",
        "FluentValidation.IValidator",
        "FastEndpoints.IPreProcessor",
        "FastEndpoints.IPostProcessor"
    );

    #endregion

    #region IIncrementalGenerator Implementation

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a provider that extracts assembly name
        var assemblyNameProvider = context.CompilationProvider
            .Select(static (compilation, _) => compilation.AssemblyName ?? "Assembly");

        // Create a provider that discovers qualifying types
        // WithTrackingName helps with debugging and IDE performance analysis
        var typesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractTypeInfo)
            .Where(static result => result is not null)
            .WithTrackingName("DiscoveredTypes")
            .Collect();

        // Combine assembly name with discovered types
        var combined = assemblyNameProvider.Combine(typesProvider);

        // Register the source output
        context.RegisterSourceOutput(combined, GenerateSource!);
    }

    #endregion

    #region Syntax Analysis

    /// <summary>
    /// Fast syntactic filter - runs on every keystroke.
    /// Only allows non-generic class declarations through for semantic analysis.
    /// </summary>
    private static bool IsCandidate(SyntaxNode node, CancellationToken cancellationToken)
    {
        // Only process non-generic class declarations
        return node is ClassDeclarationSyntax { TypeParameterList: null };
    }

    /// <summary>
    /// Semantic transform - extracts type information for qualifying types.
    /// </summary>
    private static string? ExtractTypeInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // Get the declared symbol for this syntax node
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Filter out types that shouldn't be discovered
        if (!IsDiscoverableType(typeSymbol))
            return null;

        // Check if the type implements any discoverable interface
        if (!ImplementsDiscoverableInterface(typeSymbol))
            return null;

        // Return the fully qualified type name
        return typeSymbol.ToDisplayString();
    }

    /// <summary>
    /// Determines if a type should be considered for discovery.
    /// </summary>
    private static bool IsDiscoverableType(INamedTypeSymbol typeSymbol)
    {
        // Must not be abstract
        if (typeSymbol.IsAbstract)
            return false;

        // Must not be file-scoped (C# 11+) - these can't be referenced from generated code
        if (typeSymbol.IsFileLocal)
            return false;

        // Must not have [DontRegister] attribute
        if (HasDontRegisterAttribute(typeSymbol))
            return false;

        // Must implement at least one interface
        if (typeSymbol.AllInterfaces.Length == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the type has the [DontRegister] attribute.
    /// </summary>
    private static bool HasDontRegisterAttribute(INamedTypeSymbol typeSymbol)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Name == DontRegisterAttribute)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the type implements any of the discoverable interfaces.
    /// </summary>
    private static bool ImplementsDiscoverableInterface(INamedTypeSymbol typeSymbol)
    {
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            var ifaceName = iface.ToDisplayString();
            if (DiscoverableInterfaces.Contains(ifaceName))
                return true;
        }
        return false;
    }

    #endregion

    #region Code Generation

    /// <summary>
    /// Generates the DiscoveredTypes source file.
    /// </summary>
    private static void GenerateSource(
        SourceProductionContext context,
        (string AssemblyName, ImmutableArray<string?> Types) data)
    {
        var (assemblyName, types) = data;

        // Get distinct, sorted type names (excluding nulls)
        var distinctTypes = types
            .Where(t => t is not null)
            .Cast<string>()
            .Distinct()
            .OrderBy(t => t)
            .ToImmutableArray();

        // Always generate the file, even if empty (provides a stable API)
        var source = GenerateDiscoveredTypesClass(assemblyName, distinctTypes);
        context.AddSource("DiscoveredTypes.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    // Assembly info for GeneratedCode attribute
    private static readonly string GeneratorName = typeof(DiscoveredTypesGenerator).FullName ?? "FastEndpoints.Generator.DiscoveredTypesGenerator";
    private static readonly string GeneratorVersion = typeof(DiscoveredTypesGenerator).Assembly.GetName().Version?.ToString() ?? "1.0.0";

    /// <summary>
    /// Generates the DiscoveredTypes class source code.
    /// </summary>
    private static string GenerateDiscoveredTypesClass(string assemblyName, ImmutableArray<string> types)
    {
        var builder = new StringBuilder(4096);

        // Auto-generated file header
        builder.AppendLine("//------------------------------------------------------------------------------");
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("//     This code was generated by FastEndpoints.Generator.");
        builder.AppendLine("//");
        builder.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
        builder.AppendLine("//     the code is regenerated.");
        builder.AppendLine("// </auto-generated>");
        builder.AppendLine("//------------------------------------------------------------------------------");
        builder.AppendLine("#pragma warning disable CS0618");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.Append("namespace ").Append(assemblyName).AppendLine(";");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.CodeDom.Compiler;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Diagnostics.CodeAnalysis;");
        builder.AppendLine();

        // Class documentation
        builder.AppendLine("/// <summary>");
        builder.AppendLine("/// Auto-generated list of discovered FastEndpoints types.");
        builder.AppendLine("/// The DynamicDependency attributes ensure AOT/trimming preserves method and constructor metadata.");
        builder.AppendLine("/// </summary>");
        builder.Append("[GeneratedCode(\"").Append(GeneratorName).Append("\", \"").Append(GeneratorVersion).AppendLine("\")]");
        builder.AppendLine("public static class DiscoveredTypes");
        builder.AppendLine("{");

        // Generate a static method with DynamicDependency attributes for AOT support
        // (DynamicDependency can only be applied to methods, constructors, or fields)
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Ensures AOT/trimming preserves public methods and constructors of discovered types.");
        builder.AppendLine("    /// This method exists solely to host the DynamicDependency attributes.");
        builder.AppendLine("    /// </summary>");
        foreach (var typeName in types)
        {
            // Skip malformed generic types (open generics that slipped through)
            if (IsOpenGenericType(typeName))
                continue;

            builder.Append("    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(");
            builder.Append(typeName);
            builder.AppendLine("))]");
        }
        builder.AppendLine("    public static void EnsureAotPreservation() { }");
        builder.AppendLine();

        // Property documentation
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// All discovered endpoint, validator, event handler, and command handler types.");
        builder.AppendLine("    /// Each type has its public methods and constructors preserved for AOT compatibility.");
        builder.AppendLine("    /// </summary>");
        builder.AppendLine("    public static readonly List<Type> All =");
        builder.AppendLine("    [");

        // Type list
        foreach (var typeName in types)
        {
            builder.Append("        typeof(");
            builder.Append(typeName);
            builder.AppendLine("),");
        }

        builder.AppendLine("    ];");
        builder.AppendLine("}");

        return builder.ToString();
    }

    /// <summary>
    /// Checks if the type name represents an open generic type (e.g., "MyClass&lt;T&gt;" without concrete type arguments).
    /// </summary>
    private static bool IsOpenGenericType(string typeName)
    {
        // Open generic types have '<' but don't end with '>' when displayed
        // This catches cases like "MyClass<T" which are malformed
        return typeName.Contains("<") && !typeName.EndsWith(">");
    }

    #endregion
}
