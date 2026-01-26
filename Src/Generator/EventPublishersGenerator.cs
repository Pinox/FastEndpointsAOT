using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FastEndpoints.Generator;

/// <summary>
/// Source generator that discovers all IEvent types (via IEventHandler&lt;TEvent&gt; implementations)
/// and generates a static EventPublishers class with pre-compiled publish delegates for AOT compatibility.
/// This eliminates the need for MakeGenericType and Expression.Compile() at runtime.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class EventPublishersGenerator : IIncrementalGenerator
{
    #region Constants

    /// <summary>
    /// The fully qualified name of the IEventHandler&lt;TEvent&gt; interface.
    /// </summary>
    private const string EventHandlerInterface = "FastEndpoints.IEventHandler`1";

    /// <summary>
    /// The fully qualified name of the IEvent interface.
    /// </summary>
    private const string EventInterface = "FastEndpoints.IEvent";

    /// <summary>
    /// Attribute that marks types to be excluded from auto-registration.
    /// </summary>
    private const string DontRegisterAttribute = "DontRegisterAttribute";

    #endregion

    #region IIncrementalGenerator Implementation

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a provider that extracts assembly name
        var assemblyNameProvider = context.CompilationProvider
            .Select(static (compilation, _) => compilation.AssemblyName ?? "Assembly");

        // Create a provider that discovers event types from IEventHandler<TEvent> implementations
        var eventTypesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractEventType)
            .Where(static result => result is not null)
            .WithTrackingName("EventTypes")
            .Collect();

        // Also discover direct IEvent implementations (in case events have no handlers registered yet)
        var directEventTypesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractDirectEventType)
            .Where(static result => result is not null)
            .WithTrackingName("DirectEventTypes")
            .Collect();

        // Combine all providers
        var combined = assemblyNameProvider
            .Combine(eventTypesProvider)
            .Combine(directEventTypesProvider);

        // Register the source output
        context.RegisterSourceOutput(combined, GenerateSource!);
    }

    #endregion

    #region Syntax Analysis

    /// <summary>
    /// Fast syntactic filter - runs on every keystroke.
    /// Only allows class declarations through for semantic analysis.
    /// </summary>
    private static bool IsCandidate(SyntaxNode node, CancellationToken cancellationToken)
    {
        // Only process class declarations (both generic and non-generic)
        return node is ClassDeclarationSyntax;
    }

    /// <summary>
    /// Extracts the TEvent type from IEventHandler&lt;TEvent&gt; implementations.
    /// </summary>
    private static string? ExtractEventType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // Get the declared symbol for this syntax node
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Filter out types that shouldn't be processed
        if (!IsValidHandlerType(typeSymbol))
            return null;

        // Find IEventHandler<TEvent> interface and extract TEvent
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface.OriginalDefinition.ToDisplayString() == EventHandlerInterface &&
                iface.TypeArguments.Length == 1)
            {
                var eventType = iface.TypeArguments[0];

                // Skip if the event type is an open generic parameter
                if (eventType.TypeKind == TypeKind.TypeParameter)
                    continue;

                // Skip if the event type is file-scoped
                if (eventType is INamedTypeSymbol namedEventType && namedEventType.IsFileLocal)
                    continue;

                return eventType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        return null;
    }

    /// <summary>
    /// Extracts direct IEvent implementations (classes that implement IEvent directly).
    /// </summary>
    private static string? ExtractDirectEventType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // Get the declared symbol for this syntax node
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Must be a concrete, non-abstract class
        if (typeSymbol.IsAbstract || typeSymbol.TypeKind != TypeKind.Class)
            return null;

        // Must not be file-scoped
        if (typeSymbol.IsFileLocal)
            return null;

        // Must not be generic
        if (typeSymbol.IsGenericType)
            return null;

        // Check if it implements IEvent directly
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == EventInterface)
            {
                return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if a type should be considered as a valid event handler.
    /// </summary>
    private static bool IsValidHandlerType(INamedTypeSymbol typeSymbol)
    {
        // Must not be abstract
        if (typeSymbol.IsAbstract)
            return false;

        // Must not be file-scoped
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

    #endregion

    #region Code Generation

    /// <summary>
    /// Generates the EventPublishers source file.
    /// </summary>
    private static void GenerateSource(
        SourceProductionContext context,
        ((string AssemblyName, ImmutableArray<string?> HandlerEventTypes) Left, ImmutableArray<string?> DirectEventTypes) data)
    {
        var assemblyName = data.Left.AssemblyName;
        var handlerEventTypes = data.Left.HandlerEventTypes;
        var directEventTypes = data.DirectEventTypes;

        // Combine event types from handlers and direct implementations, remove duplicates
        // Use StringComparer.Ordinal for deterministic, culture-invariant comparison
        var allEventTypes = handlerEventTypes
            .Concat(directEventTypes)
            .Where(static t => t is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static t => t, StringComparer.Ordinal)
            .ToImmutableArray();

        // Only generate if we have event types
        if (allEventTypes.Length == 0)
            return;

        var source = GenerateEventPublishersClass(assemblyName, allEventTypes);
        context.AddSource("EventPublishers.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    // Assembly info for GeneratedCode attribute
    private static readonly string GeneratorName = typeof(EventPublishersGenerator).FullName ?? "FastEndpoints.Generator.EventPublishersGenerator";
    private static readonly string GeneratorVersion = typeof(EventPublishersGenerator).Assembly.GetName().Version?.ToString() ?? "1.0.0";

    /// <summary>
    /// Generates the EventPublishers class source code.
    /// </summary>
    private static string GenerateEventPublishersClass(string assemblyName, ImmutableArray<string> eventTypes)
    {
        // Generate dynamic sections
        var dynamicDependencyAttrs = string.Join("\n", eventTypes.Select(t =>
            $"    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(global::FastEndpoints.EventBus<{t}>))]"));

        var publisherEntries = string.Join("\n", eventTypes.Select(t =>
            $"        [typeof({t})] = Publish_{GetSafeMethodName(t)},"));

        var publishMethods = string.Join("\n", eventTypes.Select(t =>
            $$"""
                  /// <summary>
                  /// Publishes an event of type <see cref="{{t.Replace("global::", "")}}"/>.
                  /// </summary>
                  private static Task Publish_{{GetSafeMethodName(t)}}(IEvent eventModel, Mode waitMode, CancellationToken cancellation)
                      => global::FastEndpoints.EventPublisherRegistry.PublishEvent(({{t}})eventModel, waitMode, cancellation);
            """));

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

            using System;
            using System.CodeDom.Compiler;
            using System.Collections.Generic;
            using System.Diagnostics.CodeAnalysis;
            using System.Runtime.CompilerServices;
            using System.Threading;
            using System.Threading.Tasks;
            using FastEndpoints;

            namespace {{assemblyName}};

            /// <summary>
            /// Module initializer that automatically registers the generated event publishers.
            /// </summary>
            file static class EventPublishersInitializer
            {
                [ModuleInitializer]
                internal static void Initialize()
                {
                    global::FastEndpoints.EventPublisherRegistry.RegisterGeneratedPublishers(EventPublishers.GetPublisher);
                }
            }

            /// <summary>
            /// Auto-generated AOT-compatible event publishers.
            /// Provides pre-compiled publish delegates for all discovered IEvent types,
            /// eliminating the need for MakeGenericType and Expression.Compile() at runtime.
            /// </summary>
            [GeneratedCode("{{GeneratorName}}", "{{GeneratorVersion}}")]
            public static class EventPublishers
            {
                /// <summary>
                /// Ensures AOT/trimming preserves EventBus&lt;T&gt; for all discovered event types.
                /// </summary>
            {{dynamicDependencyAttrs}}
                public static void EnsureAotPreservation() { }

                /// <summary>
                /// Pre-compiled publish delegates keyed by event type.
                /// </summary>
                private static readonly Dictionary<Type, Func<IEvent, Mode, CancellationToken, Task>> Publishers = new()
                {
            {{publisherEntries}}
                };

                /// <summary>
                /// Gets the pre-compiled publish delegate for the specified event type.
                /// Returns null if no publisher was generated for this event type.
                /// </summary>
                /// <param name="eventType">The type of the event.</param>
                /// <returns>The publish delegate, or null if not found.</returns>
                public static Func<IEvent, Mode, CancellationToken, Task>? GetPublisher(Type eventType)
                    => Publishers.TryGetValue(eventType, out var publisher) ? publisher : null;

                /// <summary>
                /// Attempts to publish an event using the pre-compiled delegate.
                /// </summary>
                /// <param name="eventModel">The event to publish.</param>
                /// <param name="waitMode">The wait mode for subscribers.</param>
                /// <param name="cancellation">Cancellation token.</param>
                /// <param name="task">The resulting task if a publisher was found.</param>
                /// <returns>True if a pre-compiled publisher was found and used.</returns>
                public static bool TryPublish(IEvent eventModel, Mode waitMode, CancellationToken cancellation, out Task? task)
                {
                    if (Publishers.TryGetValue(eventModel.GetType(), out var publisher))
                    {
                        task = publisher(eventModel, waitMode, cancellation);
                        return true;
                    }
                    task = null;
                    return false;
                }

            {{publishMethods}}
            }
            """;
    }

    /// <summary>
    /// Converts a fully qualified type name to a safe method name.
    /// </summary>
    private static string GetSafeMethodName(string typeName)
    {
        // Remove global:: prefix
        var name = typeName;
        if (name.StartsWith("global::"))
            name = name.Substring(8);

        // Replace dots, angle brackets, commas, and spaces with underscores
        return name
            .Replace(".", "_")
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace(",", "_")
            .Replace(" ", "");
    }

    #endregion
}
