using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FastEndpoints.Generator;

/// <summary>
/// Source generator that pre-generates reflection data for request DTOs and command handlers.
/// This eliminates runtime reflection and enables AOT compatibility by:
/// 1. Pre-computing object factories, property setters, and value parsers for request binding
/// 2. Pre-generating command handler executors to avoid MakeGenericType at runtime
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ReflectionGenerator : IIncrementalGenerator
{
    #region Constants

    // Type blacklist - types that should not have reflection data generated
    private static readonly ImmutableHashSet<string> TypeBlacklist = ImmutableHashSet.Create(
        "Microsoft.Extensions.Primitives.StringSegment",
        "FastEndpoints.EmptyRequest",
        "System.Uri"
    );

    // Attribute names
    private const string ConditionArgument = "Condition";
    private const string DontInjectAttribute = "DontInjectAttribute";
    private const string DontRegisterAttribute = "DontRegisterAttribute";
    private const string JsonIgnoreAttribute = "System.Text.Json.Serialization.JsonIgnoreAttribute";

    // Interface patterns
    private const string FluentGenericEndpoint = "FastEndpoints.Ep.";
    private const string IEndpoint = "FastEndpoints.IEndpoint";
    private const string IEnumerable = "System.Collections.IEnumerable";
    private const string IParsablePrefix = "System.IParsable<";
    private const string ICommandHandlerPrefix = "FastEndpoints.ICommandHandler<";

    // Void type for command handlers without a result
    private const string VoidResultType = "global::FastEndpoints.Void";

    #endregion

    #region IIncrementalGenerator Implementation

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get assembly name
        var assemblyNameProvider = context.CompilationProvider
            .Select(static (compilation, _) => compilation.AssemblyName ?? "Assembly");

        // Discover endpoints and their request DTOs
        // WithComparer enables Roslyn's incremental caching - only re-runs Generate when actual data changes
        // WithTrackingName helps with debugging and IDE performance analysis
        var endpointsProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractEndpointInfo)
            .Where(static info => info is not null)
            .WithComparer(EndpointInfoComparer.Instance)
            .WithTrackingName("EndpointDiscovery")
            .Collect();

        // Discover command handlers
        var commandHandlersProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidate,
                transform: ExtractCommandHandlerInfo)
            .Where(static info => info is not null)
            .WithComparer(CommandHandlerInfoComparer.Instance)
            .WithTrackingName("CommandHandlerDiscovery")
            .Collect();

        // Combine all providers
        var combined = assemblyNameProvider
            .Combine(endpointsProvider)
            .Combine(commandHandlersProvider);

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
        return node is ClassDeclarationSyntax { TypeParameterList: null };
    }

    /// <summary>
    /// Extracts endpoint information if the type is an endpoint.
    /// </summary>
    private static EndpointInfo? ExtractEndpointInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Check basic eligibility
        if (!IsEligibleType(typeSymbol))
            return null;

        // Check if it implements IEndpoint
        if (!ImplementsInterface(typeSymbol, IEndpoint))
            return null;

        // Extract the request DTO type from the endpoint's base class
        var requestType = ExtractRequestTypeFromEndpoint(typeSymbol);
        if (requestType is null)
            return null;

        return new EndpointInfo(typeSymbol, requestType);
    }

    /// <summary>
    /// Extracts command handler information if the type is a command handler.
    /// </summary>
    private static CommandHandlerInfo? ExtractCommandHandlerInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        // Check basic eligibility
        if (!IsEligibleType(typeSymbol))
            return null;

        // Find the ICommandHandler<,> interface
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            var ifaceDisplay = iface.ToDisplayString();
            if (!ifaceDisplay.StartsWith(ICommandHandlerPrefix, StringComparison.Ordinal))
                continue;

            if (iface is not INamedTypeSymbol { TypeArguments.Length: >= 1 } namedIfc)
                continue;

            var commandType = namedIfc.TypeArguments[0];
            var resultType = namedIfc.TypeArguments.Length > 1 ? namedIfc.TypeArguments[1] : null;

            // Use FullyQualifiedFormat to get proper CLR type names (e.g., System.String instead of string)
            var resultTypeName = resultType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? VoidResultType;

            return new CommandHandlerInfo(
                typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                commandType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                resultTypeName);
        }

        return null;
    }

    /// <summary>
    /// Checks if a type is eligible for processing (not abstract, not file-local, etc.).
    /// </summary>
    private static bool IsEligibleType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAbstract)
            return false;

        if (typeSymbol.IsFileLocal)
            return false;

        if (typeSymbol.AllInterfaces.Length == 0)
            return false;

        // Check for [DontRegister] attribute
        foreach (var attr in typeSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name == DontRegisterAttribute)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a type implements a specific interface.
    /// </summary>
    private static bool ImplementsInterface(INamedTypeSymbol typeSymbol, string interfaceName)
    {
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == interfaceName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Extracts the request DTO type from an endpoint's base class.
    /// </summary>
    private static ITypeSymbol? ExtractRequestTypeFromEndpoint(INamedTypeSymbol endpointSymbol)
    {
        var baseType = endpointSymbol.BaseType;

        // Handle fluent generic endpoints (Ep.Req<T>.Res<R>)
        if (baseType?.ToDisplayString().StartsWith(FluentGenericEndpoint, StringComparison.Ordinal) == true)
            baseType = baseType.BaseType;

        // Walk up the inheritance chain to find the type argument
        while (baseType is not null)
        {
            if (baseType.TypeArguments.Length > 0)
                return baseType.TypeArguments[0];

            baseType = baseType.BaseType;
        }

        return null;
    }

    #endregion

    #region Code Generation

    /// <summary>
    /// Generates the ReflectionData source file.
    /// </summary>
    private static void GenerateSource(
        SourceProductionContext context,
        ((string AssemblyName, ImmutableArray<EndpointInfo?> Endpoints), ImmutableArray<CommandHandlerInfo?> CommandHandlers) data)
    {
        var ((assemblyName, endpoints), commandHandlers) = data;

        // Collect all type information for reflection data generation
        var collector = new TypeCollector();
        
        // Process endpoints
        foreach (var endpoint in endpoints)
        {
            if (endpoint is null) continue;
            ProcessTypeForReflection(collector, endpoint.RequestType, isFromEndpoint: true);
        }

        // Get distinct command handlers (by CommandType)
        var distinctHandlers = commandHandlers
            .Where(h => h is not null)
            .Cast<CommandHandlerInfo>()
            .GroupBy(h => h.CommandType)
            .Select(g => g.First())
            .ToImmutableArray();

        // Generate the source
        var source = GenerateReflectionClass(assemblyName, collector, distinctHandlers);
        context.AddSource("ReflectionData.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// Processes a type for reflection data generation.
    /// </summary>
    private static void ProcessTypeForReflection(TypeCollector collector, ITypeSymbol typeSymbol, bool isFromEndpoint)
    {
        // Skip if already processed, abstract, enum, or interface
        if (typeSymbol.IsAbstract || typeSymbol.TypeKind == TypeKind.Enum || typeSymbol.TypeKind == TypeKind.Interface)
            return;

        // Get the underlying type (handles nullable)
        typeSymbol = typeSymbol.GetUnderlyingType();

        var typeName = typeSymbol.ToDisplayString();
        var underlyingTypeName = typeSymbol.ToDisplayString(NullableFlowState.None);

        // Skip blacklisted types
        if (TypeBlacklist.Contains(underlyingTypeName))
            return;

        // Skip if already collected
        if (collector.HasType(typeName))
            return;

        // Check for IEnumerable - process element type instead
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == IEnumerable)
            {
                var elementType = GetEnumerableElementType(typeSymbol);
                if (elementType is not null)
                    ProcessTypeForReflection(collector, elementType, isFromEndpoint: false);
                return;
            }
        }

        // Collect type information
        var typeInfo = CollectTypeInfo(collector, typeSymbol, isFromEndpoint);
        if (typeInfo is not null)
        {
            collector.Add(typeInfo);

            // Recursively process property types
            foreach (var prop in typeInfo.Properties)
            {
                ProcessTypeForReflection(collector, prop.TypeSymbol, isFromEndpoint: false);
            }
        }
    }

    /// <summary>
    /// Collects detailed type information for reflection data generation.
    /// </summary>
    private static TypeReflectionInfo? CollectTypeInfo(TypeCollector collector, ITypeSymbol typeSymbol, bool isFromEndpoint)
    {
        var typeName = typeSymbol.ToDisplayString();
        var underlyingTypeName = typeSymbol.ToDisplayString(NullableFlowState.None);
        var isValueType = typeSymbol.IsValueType;

        // Determine if the type is parsable
        bool? isParsable = null;
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface.ToDisplayString().StartsWith(IParsablePrefix, StringComparison.Ordinal))
            {
                isParsable = true;
                break;
            }
        }

        // Collect properties and constructor info
        var properties = new List<PropertyReflectionInfo>();
        var ctorArgumentCount = 0;
        var ctorSearchComplete = false;
        var currentSymbol = typeSymbol;

        while (currentSymbol is not null)
        {
            foreach (var member in currentSymbol.GetMembers())
            {
                switch (member)
                {
                    // Check for TryParse method (indicates non-IParsable parsability)
                    case IMethodSymbol method when IsTryParseMethod(method, currentSymbol):
                        isParsable = false;
                        break;

                    // Collect constructor info
                    case IMethodSymbol { MethodKind: MethodKind.Constructor, DeclaredAccessibility: Accessibility.Public, IsStatic: false } ctor:
                        if (!ctorSearchComplete)
                        {
                            var argCount = ctor.Parameters.Count(p => !p.HasExplicitDefaultValue);
                            if (ctorArgumentCount == 0 || (argCount > 0 && ctorArgumentCount > argCount))
                                ctorArgumentCount = argCount;
                        }
                        break;

                    // Collect property info
                    case IPropertySymbol { DeclaredAccessibility: Accessibility.Public, IsStatic: false } prop
                        when prop.GetMethod?.DeclaredAccessibility == Accessibility.Public &&
                             prop.SetMethod?.DeclaredAccessibility == Accessibility.Public:

                        // Skip properties with [DontInject] or [JsonIgnore] unless required
                        if ((HasDontInjectAttribute(prop) || HasUnconditionalJsonIgnoreAttribute(prop)) && !prop.IsRequired)
                            break;

                        properties.Add(new PropertyReflectionInfo(
                            prop.Name,
                            prop.Type,
                            prop.Type.ToDisplayString(),
                            prop.SetMethod?.IsInitOnly == true,
                            prop.IsRequired));
                        break;
                }
            }

            ctorSearchComplete = true;
            currentSymbol = currentSymbol.BaseType;
        }

        // Skip types with no properties
        if (properties.Count == 0)
            return null;

        // Generate a hash code for change detection
        var hashCode = typeSymbol.DeclaringSyntaxReferences.Length > 0
            ? typeSymbol.DeclaringSyntaxReferences[0].Span.Length
            : 0;

        return new TypeReflectionInfo(
            $"t{collector.Counter}",
            typeName,
            underlyingTypeName,
            isValueType,
            isParsable,
            ctorArgumentCount,
            properties.ToImmutableArray(),
            hashCode,
            !isFromEndpoint);
    }

    /// <summary>
    /// Checks if a method is a valid TryParse method.
    /// </summary>
    private static bool IsTryParseMethod(IMethodSymbol method, ITypeSymbol containingType)
    {
        return method.Name == "TryParse" &&
               method.DeclaredAccessibility == Accessibility.Public &&
               method.IsStatic &&
               method.ReturnType.SpecialType == SpecialType.System_Boolean &&
               method.Parameters.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               method.Parameters[1].RefKind == RefKind.Out &&
               method.Parameters[1].Type.Name == containingType.Name;
    }

    /// <summary>
    /// Checks if a property has the [DontInject] attribute.
    /// </summary>
    private static bool HasDontInjectAttribute(IPropertySymbol prop)
    {
        foreach (var attr in prop.GetAttributes())
        {
            if (attr.AttributeClass?.Name == DontInjectAttribute)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a property has an unconditional [JsonIgnore] attribute.
    /// </summary>
    private static bool HasUnconditionalJsonIgnoreAttribute(IPropertySymbol prop)
    {
        foreach (var attr in prop.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() != JsonIgnoreAttribute)
                continue;

            // Check for Condition argument
            foreach (var namedArg in attr.NamedArguments)
            {
                if (namedArg.Key != ConditionArgument)
                    continue;

                // JsonIgnoreCondition.Always = 1
                if (namedArg.Value.Value is int conditionValue && conditionValue == 1)
                    return true;
            }

            // No condition means always ignore
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the element type from an enumerable type.
    /// </summary>
    private static ITypeSymbol? GetEnumerableElementType(ITypeSymbol typeSymbol)
    {
        return typeSymbol switch
        {
            IArrayTypeSymbol arrayType => arrayType.ElementType,
            INamedTypeSymbol { TypeArguments.Length: > 0 } namedType => namedType.TypeArguments[0],
            _ => null
        };
    }

    /// <summary>
    /// Generates the complete ReflectionData class source code.
    /// </summary>
    private static string GenerateReflectionClass(
        string assemblyName,
        TypeCollector collector,
        ImmutableArray<CommandHandlerInfo> commandHandlers)
    {
        var sanitizedAssemblyName = assemblyName.Sanitize(string.Empty);
        var builder = new StringBuilder(8192);

        // Generate file header
        GenerateFileHeader(builder, collector);

        // Generate namespace and class
        builder.Append("namespace ").Append(assemblyName).AppendLine(";");
        builder.AppendLine();
        builder.AppendLine("/// <summary>");
        builder.Append("/// Source generated reflection data for request DTOs in the [").Append(assemblyName).AppendLine("] assembly.");
        builder.AppendLine("/// </summary>");
        builder.Append("[GeneratedCode(\"").Append(GeneratorName).Append("\", \"").Append(GeneratorVersion).AppendLine("\")]");
        builder.AppendLine("public static class GeneratedReflection");
        builder.AppendLine("{");

        // Generate AddFrom method
        GenerateAddFromMethod(builder, sanitizedAssemblyName, collector);

        // Generate RegisterCommandExecutors method
        GenerateRegisterCommandExecutorsMethod(builder, commandHandlers);

        builder.AppendLine("}");

        return builder.ToString();
    }

    // Assembly info for GeneratedCode attribute
    private static readonly string GeneratorName = typeof(ReflectionGenerator).FullName ?? "FastEndpoints.Generator.ReflectionGenerator";
    private static readonly string GeneratorVersion = typeof(ReflectionGenerator).Assembly.GetName().Version?.ToString() ?? "1.0.0";

    /// <summary>
    /// Generates the file header with using directives and type aliases.
    /// </summary>
    private static void GenerateFileHeader(StringBuilder builder, TypeCollector collector)
    {
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
        builder.AppendLine("using System.CodeDom.Compiler;");
        builder.AppendLine("using FastEndpoints;");
        builder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        builder.AppendLine("using System.Globalization;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine();

        // Generate type aliases
        foreach (var typeInfo in collector.Types)
        {
            builder.Append("using ").Append(typeInfo.TypeAlias).Append(" = ").Append(typeInfo.UnderlyingTypeName).AppendLine(";");
        }

        builder.AppendLine();
    }

    /// <summary>
    /// Generates the AddFrom extension method for ReflectionCache.
    /// </summary>
    private static void GenerateAddFromMethod(StringBuilder builder, string sanitizedAssemblyName, TypeCollector collector)
    {
        builder.AppendLine("    /// <summary>");
        builder.Append("    /// Register source generated reflection data from [").Append(sanitizedAssemblyName).AppendLine("] with the central cache.");
        builder.AppendLine("    /// </summary>");
        builder.Append("    public static ReflectionCache AddFrom").Append(sanitizedAssemblyName).AppendLine("(this ReflectionCache cache)");
        builder.AppendLine("    {");

        foreach (var typeInfo in collector.Types)
        {
            GenerateTypeRegistration(builder, typeInfo);
        }

        builder.AppendLine("        return cache;");
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    /// <summary>
    /// Generates the registration code for a single type.
    /// </summary>
    private static void GenerateTypeRegistration(StringBuilder builder, TypeReflectionInfo typeInfo)
    {
        builder.Append("        // ").AppendLine(typeInfo.UnderlyingTypeName);
        builder.Append("        var _").Append(typeInfo.TypeAlias).Append(" = typeof(").Append(typeInfo.TypeAlias).AppendLine(");");
        builder.AppendLine("        cache.TryAdd(");
        builder.Append("            _").Append(typeInfo.TypeAlias).AppendLine(",");
        builder.AppendLine("            new()");
        builder.AppendLine("            {");

        // Generate ObjectFactory if needed
        if (!typeInfo.SkipObjectFactory)
        {
            var ctorArgs = typeInfo.CtorArgumentCount > 0
                ? string.Join(", ", Enumerable.Repeat("default!", typeInfo.CtorArgumentCount))
                : string.Empty;
            var initArgs = GenerateInitializerArgs(typeInfo.Properties.Where(p => p.IsRequired).Select(p => p.Name));

            builder.Append("                ObjectFactory = () => new ").Append(typeInfo.TypeAlias);
            builder.Append("(").Append(ctorArgs).Append(")").Append(initArgs).AppendLine(",");
        }

        // Generate ValueParser if needed
        if (typeInfo.IsParsable is not null)
        {
            var tryParseArgs = typeInfo.IsParsable == true
                ? "input, CultureInfo.InvariantCulture, out var result"
                : "input, out var result";

            builder.Append("                ValueParser = input => new(").Append(typeInfo.TypeAlias);
            builder.Append(".TryParse(").Append(tryParseArgs).AppendLine("), result),");
        }

        // Generate Properties if any
        if (typeInfo.Properties.Length > 0)
        {
            builder.AppendLine("                Properties = new(");
            builder.AppendLine("                [");

            foreach (var prop in typeInfo.Properties)
            {
                var propCast = typeInfo.IsValueType
                    ? $"Unsafe.Unbox<{typeInfo.TypeAlias}>(dto)"
                    : $"(({typeInfo.TypeAlias})dto)";

                builder.Append("                    new(_").Append(typeInfo.TypeAlias);
                builder.Append(".GetProperty(nameof(").Append(typeInfo.TypeAlias).Append(".").Append(prop.Name).Append("))!, ");

                if (prop.IsInitOnly)
                {
                    builder.AppendLine("new()),");
                }
                else
                {
                    builder.Append("new() { Setter = (dto, val) => ").Append(propCast).Append(".");
                    builder.Append(prop.Name).Append(" = (").Append(prop.TypeName).AppendLine(")val! }),");
                }
            }

            builder.AppendLine("                ])");
        }

        builder.AppendLine("            });");
        builder.AppendLine();
    }

    /// <summary>
    /// Generates the RegisterCommandExecutors method.
    /// </summary>
    private static void GenerateRegisterCommandExecutorsMethod(StringBuilder builder, ImmutableArray<CommandHandlerInfo> commandHandlers)
    {
        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// Register pre-generated command handler executors from the source generator.");
        builder.AppendLine("    /// This enables AOT compatibility by avoiding MakeGenericType at runtime.");
        builder.AppendLine("    /// </summary>");
        builder.AppendLine("    public static void RegisterCommandExecutors(CommandHandlerRegistry registry, IServiceProvider sp)");
        builder.AppendLine("    {");

        foreach (var handler in commandHandlers)
        {
            builder.Append("        // ").Append(handler.CommandType).Append(" -> ").AppendLine(handler.HandlerType);
            builder.Append("        registry[typeof(").Append(handler.CommandType).Append(")] = new(typeof(").Append(handler.HandlerType).AppendLine("))");
            builder.AppendLine("        {");
            builder.Append("            HandlerExecutor = new global::FastEndpoints.CommandHandlerExecutor<");
            builder.Append(handler.CommandType).Append(", ").Append(handler.ResultType).AppendLine(">(");
            builder.Append("                sp.GetService<global::System.Collections.Generic.IEnumerable<global::FastEndpoints.ICommandMiddleware<");
            builder.Append(handler.CommandType).Append(", ").Append(handler.ResultType).AppendLine(">>>()");
            builder.Append("                ?? global::System.Array.Empty<global::FastEndpoints.ICommandMiddleware<");
            builder.Append(handler.CommandType).Append(", ").Append(handler.ResultType).AppendLine(">>())");
            builder.AppendLine("        };");
            builder.AppendLine();
        }

        builder.AppendLine("    }");
    }

    /// <summary>
    /// Generates object initializer arguments for required properties.
    /// </summary>
    private static string GenerateInitializerArgs(IEnumerable<string> propertyNames)
    {
        var props = propertyNames.ToList();
        if (props.Count == 0)
            return string.Empty;

        var builder = new StringBuilder(" { ");
        foreach (var prop in props)
        {
            builder.Append(prop).Append(" = default!, ");
        }
        builder.Remove(builder.Length - 2, 2);
        builder.Append(" }");
        return builder.ToString();
    }

    #endregion

    #region Data Types

    /// <summary>
    /// Represents an endpoint discovered during source generation.
    /// </summary>
    private sealed class EndpointInfo
    {
        public INamedTypeSymbol EndpointType { get; }
        public ITypeSymbol RequestType { get; }

        public EndpointInfo(INamedTypeSymbol endpointType, ITypeSymbol requestType)
        {
            EndpointType = endpointType;
            RequestType = requestType;
        }
    }

    /// <summary>
    /// Represents a command handler discovered during source generation.
    /// </summary>
    private sealed class CommandHandlerInfo
    {
        public string HandlerType { get; }
        public string CommandType { get; }
        public string ResultType { get; }

        public CommandHandlerInfo(string handlerType, string commandType, string resultType)
        {
            HandlerType = handlerType;
            CommandType = commandType;
            ResultType = resultType;
        }
    }

    /// <summary>
    /// Represents reflection information for a type.
    /// </summary>
    private sealed class TypeReflectionInfo
    {
        public string TypeAlias { get; }
        public string TypeName { get; }
        public string UnderlyingTypeName { get; }
        public bool IsValueType { get; }
        public bool? IsParsable { get; }
        public int CtorArgumentCount { get; }
        public ImmutableArray<PropertyReflectionInfo> Properties { get; }
        public int HashCode { get; }
        public bool SkipObjectFactory { get; }

        public TypeReflectionInfo(
            string typeAlias,
            string typeName,
            string underlyingTypeName,
            bool isValueType,
            bool? isParsable,
            int ctorArgumentCount,
            ImmutableArray<PropertyReflectionInfo> properties,
            int hashCode,
            bool skipObjectFactory)
        {
            TypeAlias = typeAlias;
            TypeName = typeName;
            UnderlyingTypeName = underlyingTypeName;
            IsValueType = isValueType;
            IsParsable = isParsable;
            CtorArgumentCount = ctorArgumentCount;
            Properties = properties;
            HashCode = hashCode;
            SkipObjectFactory = skipObjectFactory;
        }

        /// <summary>
        /// Creates a copy with a new TypeAlias.
        /// </summary>
        public TypeReflectionInfo WithTypeAlias(string newTypeAlias)
        {
            return new TypeReflectionInfo(
                newTypeAlias,
                TypeName,
                UnderlyingTypeName,
                IsValueType,
                IsParsable,
                CtorArgumentCount,
                Properties,
                HashCode,
                SkipObjectFactory);
        }
    }

    /// <summary>
    /// Represents reflection information for a property.
    /// </summary>
    private sealed class PropertyReflectionInfo
    {
        public string Name { get; }
        public ITypeSymbol TypeSymbol { get; }
        public string TypeName { get; }
        public bool IsInitOnly { get; }
        public bool IsRequired { get; }

        public PropertyReflectionInfo(
            string name,
            ITypeSymbol typeSymbol,
            string typeName,
            bool isInitOnly,
            bool isRequired)
        {
            Name = name;
            TypeSymbol = typeSymbol;
            TypeName = typeName;
            IsInitOnly = isInitOnly;
            IsRequired = isRequired;
        }
    }

    /// <summary>
    /// Collects types for reflection data generation.
    /// </summary>
    private sealed class TypeCollector
    {
        private readonly Dictionary<string, TypeReflectionInfo> _types = new Dictionary<string, TypeReflectionInfo>();

        public int Counter => _types.Count;

        public IEnumerable<TypeReflectionInfo> Types => _types.Values;

        public bool HasType(string typeName) => _types.ContainsKey(typeName);

        public void Add(TypeReflectionInfo typeInfo)
        {
            if (!_types.ContainsKey(typeInfo.TypeName))
            {
                _types[typeInfo.TypeName] = typeInfo.WithTypeAlias($"t{_types.Count}");
            }
        }
    }

    #endregion

    #region Equality Comparers

    /// <summary>
    /// Comparer for EndpointInfo to enable incremental caching.
    /// Only re-generates code when the endpoint's request type actually changes.
    /// </summary>
    private sealed class EndpointInfoComparer : IEqualityComparer<EndpointInfo?>
    {
        internal static EndpointInfoComparer Instance { get; } = new EndpointInfoComparer();

        private EndpointInfoComparer() { }

        public bool Equals(EndpointInfo? x, EndpointInfo? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            // Compare by endpoint type name and request type name
            return x.EndpointType.ToDisplayString().Equals(y.EndpointType.ToDisplayString(), StringComparison.Ordinal) &&
                   x.RequestType.ToDisplayString().Equals(y.RequestType.ToDisplayString(), StringComparison.Ordinal);
        }

        public int GetHashCode(EndpointInfo? obj)
        {
            if (obj is null)
                return 0;

            unchecked
            {
                var hash = 17;
                hash = hash * 31 + obj.EndpointType.ToDisplayString().GetHashCode();
                hash = hash * 31 + obj.RequestType.ToDisplayString().GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>
    /// Comparer for CommandHandlerInfo to enable incremental caching.
    /// Only re-generates code when the command handler actually changes.
    /// </summary>
    private sealed class CommandHandlerInfoComparer : IEqualityComparer<CommandHandlerInfo?>
    {
        internal static CommandHandlerInfoComparer Instance { get; } = new CommandHandlerInfoComparer();

        private CommandHandlerInfoComparer() { }

        public bool Equals(CommandHandlerInfo? x, CommandHandlerInfo? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return x.HandlerType.Equals(y.HandlerType, StringComparison.Ordinal) &&
                   x.CommandType.Equals(y.CommandType, StringComparison.Ordinal) &&
                   x.ResultType.Equals(y.ResultType, StringComparison.Ordinal);
        }

        public int GetHashCode(CommandHandlerInfo? obj)
        {
            if (obj is null)
                return 0;

            unchecked
            {
                var hash = 17;
                hash = hash * 31 + obj.HandlerType.GetHashCode();
                hash = hash * 31 + obj.CommandType.GetHashCode();
                hash = hash * 31 + obj.ResultType.GetHashCode();
                return hash;
            }
        }
    }

    #endregion
}

