using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace FastEndpoints.Generator;

/// <summary>
/// Discovers FastEndpoints types at compile time and generates a static list for AOT compatibility.
/// Uses the Preserve&lt;T&gt;() pattern with [DynamicallyAccessedMembers] to instruct the AOT linker.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DiscoveredTypesGenerator : IIncrementalGenerator
{
    #region Constants

    /// <summary>
    /// Attribute that excludes types from auto-registration.
    /// </summary>
    const string DontRegisterAttribute = "DontRegisterAttribute";

    /// <summary>
    /// Interfaces that qualify a type for discovery. Types implementing any of these will be included.
    /// IPreProcessor/IPostProcessor included for AOT (open generics can't be closed at runtime).
    /// </summary>
    static readonly ImmutableHashSet<string> DiscoverableInterfaces = ImmutableHashSet.Create(
        StringComparer.Ordinal,
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
        // Extract assembly name from compilation (thread-safe via pipeline)
        var assemblyNameProvider = context.CompilationProvider
            .Select(static (compilation, _) => compilation.AssemblyName ?? "Assembly")
            .WithTrackingName("AssemblyName");

        // Discover qualifying types with caching
        var typesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractTypeName)
            .Where(static typeName => typeName is not null)
            .WithTrackingName("DiscoveredTypes")
            .Collect();

        // Combine and generate
        var combined = assemblyNameProvider.Combine(typesProvider);
        context.RegisterSourceOutput(combined, GenerateSource!);
    }

    #endregion

    #region Syntax Analysis

    /// <summary>
    /// Fast syntactic filter - runs on every keystroke.
    /// Only allows non-generic class declarations through to semantic analysis.
    /// </summary>
    static bool IsCandidate(SyntaxNode node, CancellationToken _)
        => node is ClassDeclarationSyntax { TypeParameterList: null };

    /// <summary>
    /// Semantic transform - extracts fully qualified type name for qualifying types.
    /// Returns null for types that don't qualify, enabling efficient filtering.
    /// </summary>
    static string? ExtractTypeName(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // Defensive null check for semantic model result
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Filter out ineligible types early
        if (!IsEligibleType(typeSymbol))
            return null;

        // Check if type implements any discoverable interface
        if (!ImplementsDiscoverableInterface(typeSymbol))
            return null;

        return typeSymbol.ToDisplayString();
    }

    /// <summary>
    /// Determines if a type is eligible for discovery.
    /// Filters out abstract types, types without interfaces, and types with [DontRegister].
    /// </summary>
    static bool IsEligibleType(INamedTypeSymbol typeSymbol)
    {
        // Must not be abstract
        if (typeSymbol.IsAbstract)
            return false;

        // Must have at least one interface
        if (typeSymbol.AllInterfaces.Length == 0)
            return false;

        // Must not have [DontRegister] attribute
        if (HasAttribute(typeSymbol, DontRegisterAttribute))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a type has a specific attribute by name.
    /// </summary>
    static bool HasAttribute(INamedTypeSymbol typeSymbol, string attributeName)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            if (string.Equals(attribute.AttributeClass?.Name, attributeName, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a type implements any of the discoverable FastEndpoints interfaces.
    /// </summary>
    static bool ImplementsDiscoverableInterface(INamedTypeSymbol typeSymbol)
    {
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (DiscoverableInterfaces.Contains(iface.ToDisplayString()))
                return true;
        }
        return false;
    }

    #endregion

    #region Code Generation

    /// <summary>
    /// Generates the DiscoveredTypes.g.cs source file with AOT preservation.
    /// </summary>
    static void GenerateSource(SourceProductionContext context, (string AssemblyName, ImmutableArray<string?> Types) data)
    {
        // Deduplicate and sort for deterministic output
        var types = data.Types
            .Where(static t => t is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static t => t, StringComparer.Ordinal)
            .ToImmutableArray();

        // No types discovered - skip generation
        if (types.Length == 0)
            return;

        var sourceCode = GenerateDiscoveredTypesClass(data.AssemblyName, types);
        context.AddSource("DiscoveredTypes.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    /// <summary>
    /// Generates the DiscoveredTypes class with the Preserve&lt;T&gt;() pattern for AOT.
    /// </summary>
    static string GenerateDiscoveredTypesClass(string assemblyName, ImmutableArray<string> types)
    {
        var preserveCalls = string.Join("\n", types.Select(t => $"        Preserve<{t}>(),"));

        return $$"""
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     This code was generated by FastEndpoints.Generator.
            //     Changes to this file may cause incorrect behavior and will be lost if
            //     the code is regenerated.
            // </auto-generated>
            //------------------------------------------------------------------------------

            #nullable enable
            #pragma warning disable CS0618

            namespace {{assemblyName}};

            using System;
            using System.Collections.Generic;
            using System.Diagnostics.CodeAnalysis;

            /// <summary>
            /// Auto-discovered FastEndpoints types with AOT preservation.
            /// </summary>
            public static class DiscoveredTypes
            {
                /// <summary>
                /// All discovered endpoint, validator, handler, and processor types.
                /// </summary>
                public static readonly List<Type> All =
                [
            {{preserveCalls}}
                ];

                /// <summary>
                /// Instructs the AOT linker to preserve public constructors and methods on T.
                /// </summary>
                static Type Preserve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>()
                    => typeof(T);
            }
            """;
    }

    #endregion
}
