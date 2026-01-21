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
        var allEventTypes = handlerEventTypes
            .Concat(directEventTypes)
            .Where(t => t is not null)
            .Cast<string>()
            .Distinct()
            .OrderBy(t => t)
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
        var builder = new StringBuilder(8192);

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
        builder.AppendLine("using System;");
        builder.AppendLine("using System.CodeDom.Compiler;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Diagnostics.CodeAnalysis;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using System.Threading;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using FastEndpoints;");
        builder.AppendLine();
        builder.Append("namespace ").Append(assemblyName).AppendLine(";");
        builder.AppendLine();

        // Generate module initializer to auto-register
        builder.AppendLine("/// <summary>");
        builder.AppendLine("/// Module initializer that automatically registers the generated event publishers.");
        builder.AppendLine("/// </summary>");
        builder.AppendLine("file static class EventPublishersInitializer");
        builder.AppendLine("{");
        builder.AppendLine("    [ModuleInitializer]");
        builder.AppendLine("    internal static void Initialize()");
        builder.AppendLine("    {");
        builder.AppendLine("        global::FastEndpoints.EventPublisherRegistry.RegisterGeneratedPublishers(EventPublishers.GetPublisher);");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();

        // Class documentation
        builder.AppendLine("/// <summary>");
        builder.AppendLine("/// Auto-generated AOT-compatible event publishers.");
        builder.AppendLine("/// Provides pre-compiled publish delegates for all discovered IEvent types,");
        builder.AppendLine("/// eliminating the need for MakeGenericType and Expression.Compile() at runtime.");
        builder.AppendLine("/// </summary>");
        builder.Append("[GeneratedCode(\"").Append(GeneratorName).Append("\", \"").Append(GeneratorVersion).AppendLine("\")]");
        builder.AppendLine("public static class EventPublishers");
        builder.AppendLine("{");

        // Generate DynamicDependency attributes method
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Ensures AOT/trimming preserves EventBus&lt;T&gt; for all discovered event types.");
        builder.AppendLine("    /// </summary>");
        foreach (var eventType in eventTypes)
        {
            builder.Append("    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicConstructors, typeof(global::FastEndpoints.EventBus<");
            builder.Append(eventType);
            builder.AppendLine(">))]");
        }
        builder.AppendLine("    public static void EnsureAotPreservation() { }");
        builder.AppendLine();

        // Generate static dictionary of publishers
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Pre-compiled publish delegates keyed by event type.");
        builder.AppendLine("    /// </summary>");
        builder.AppendLine("    private static readonly Dictionary<Type, Func<IEvent, Mode, CancellationToken, Task>> Publishers = new()");
        builder.AppendLine("    {");
        foreach (var eventType in eventTypes)
        {
            var methodName = GetSafeMethodName(eventType);
            builder.Append("        [typeof(");
            builder.Append(eventType);
            builder.Append(")] = Publish_");
            builder.Append(methodName);
            builder.AppendLine(",");
        }
        builder.AppendLine("    };");
        builder.AppendLine();

        // Generate GetPublisher method
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Gets the pre-compiled publish delegate for the specified event type.");
        builder.AppendLine("    /// Returns null if no publisher was generated for this event type.");
        builder.AppendLine("    /// </summary>");
        builder.AppendLine("    /// <param name=\"eventType\">The type of the event.</param>");
        builder.AppendLine("    /// <returns>The publish delegate, or null if not found.</returns>");
        builder.AppendLine("    public static Func<IEvent, Mode, CancellationToken, Task>? GetPublisher(Type eventType)");
        builder.AppendLine("        => Publishers.TryGetValue(eventType, out var publisher) ? publisher : null;");
        builder.AppendLine();

        // Generate TryPublish method
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Attempts to publish an event using the pre-compiled delegate.");
        builder.AppendLine("    /// </summary>");
        builder.AppendLine("    /// <param name=\"eventModel\">The event to publish.</param>");
        builder.AppendLine("    /// <param name=\"waitMode\">The wait mode for subscribers.</param>");
        builder.AppendLine("    /// <param name=\"cancellation\">Cancellation token.</param>");
        builder.AppendLine("    /// <param name=\"task\">The resulting task if a publisher was found.</param>");
        builder.AppendLine("    /// <returns>True if a pre-compiled publisher was found and used.</returns>");
        builder.AppendLine("    public static bool TryPublish(IEvent eventModel, Mode waitMode, CancellationToken cancellation, out Task? task)");
        builder.AppendLine("    {");
        builder.AppendLine("        if (Publishers.TryGetValue(eventModel.GetType(), out var publisher))");
        builder.AppendLine("        {");
        builder.AppendLine("            task = publisher(eventModel, waitMode, cancellation);");
        builder.AppendLine("            return true;");
        builder.AppendLine("        }");
        builder.AppendLine("        task = null;");
        builder.AppendLine("        return false;");
        builder.AppendLine("    }");
        builder.AppendLine();

        // Generate individual publish methods
        foreach (var eventType in eventTypes)
        {
            var methodName = GetSafeMethodName(eventType);
            builder.AppendLine("    /// <summary>");
            builder.Append("    /// Publishes an event of type <see cref=\"");
            builder.Append(eventType.Replace("global::", ""));
            builder.AppendLine("\"/>.");
            builder.AppendLine("    /// </summary>");
            builder.Append("    private static Task Publish_");
            builder.Append(methodName);
            builder.AppendLine("(IEvent eventModel, Mode waitMode, CancellationToken cancellation)");
            // Use EventPublisherRegistry.PublishEvent<T> which doesn't have naming conflicts
            builder.Append("        => global::FastEndpoints.EventPublisherRegistry.PublishEvent((");
            builder.Append(eventType);
            builder.AppendLine(")eventModel, waitMode, cancellation);");
            builder.AppendLine();
        }

        builder.AppendLine("}");

        return builder.ToString();
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
