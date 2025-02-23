using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ApolloGraphQL.Federation.HotChocolate.Constants;
using HotChocolate.Language;
using HotChocolate.Types.Introspection;
using HotChocolate.Utilities;
using HotChocolate.Utilities.Introspection;

namespace ApolloGraphQL.Federation.HotChocolate;

/// <summary>
/// The apollo federation schema printer.
/// </summary>
public static partial class FederationSchemaPrinter
{
    private static readonly HashSet<string> _builtInDirectives = new()
    {
        WellKnownTypeNames.External,
        WellKnownTypeNames.Requires,
        WellKnownTypeNames.Provides,
        WellKnownTypeNames.Key,
        WellKnownDirectives.Defer,
        WellKnownDirectives.Stream,
        WellKnownDirectives.Skip,
        WellKnownDirectives.Include,
        WellKnownDirectives.Deprecated,
        SpecifiedByDirectiveType.Names.SpecifiedBy
    };

    /// <summary>
    /// Creates a <see cref="string" /> representation of the given
    /// <paramref name="schema"/>.
    /// </summary>
    /// <param name="schema">
    /// The schema object.
    /// </param>
    /// <returns>
    /// Returns the <see cref="string" /> representation of the given
    /// <paramref name="schema"/>.
    /// </returns>
    public static string Print(ISchema schema)
    {
        if (schema is null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        return SerializeSchema(schema).ToString();
    }

    private static DocumentNode SerializeSchema(ISchema schema)
    {
        var context = new Context();
        var definitionNodes = new List<IDefinitionNode>();

        // don't add any directives
        // federation directives were already manually added

        foreach (var namedType in GetRelevantTypes(schema))
        {
            if (TrySerializeType(namedType, context, out var definitionNode))
            {
                definitionNodes.Add(definitionNode);
            }
        }

        return new DocumentNode(null, definitionNodes);
    }

    private static IEnumerable<INamedType> GetRelevantTypes(ISchema schema)
        => schema.Types
            .Where(IncludeType)
            .OrderBy(t => t.Name, StringComparer.Ordinal);

    private static bool TrySerializeType(
        INamedType namedType,
        Context context,
        [NotNullWhen(true)] out IDefinitionNode? definitionNode)
    {
        definitionNode = namedType switch
        {
            ObjectType type => SerializeObjectType(type, context),
            InterfaceType type => SerializeInterfaceType(type, context),
            InputObjectType type => SerializeInputObjectType(type, context),
            UnionType type => SerializeUnionType(type, context),
            EnumType type => SerializeEnumType(type, context),
            ScalarType type => SerializeScalarType(type, context),
            _ => throw new NotSupportedException()
        };
        return definitionNode is not null;
    }

    private static ITypeNode SerializeType(
        IType type,
        Context context)
    {
        return type switch
        {
            NonNullType nt => new NonNullTypeNode(
                (INullableTypeNode)SerializeType(nt.Type, context)),
            ListType lt => new ListTypeNode(SerializeType(lt.ElementType, context)),
            INamedType namedType => SerializeNamedType(namedType, context),
            _ => throw new NotSupportedException()
        };
    }

    private static NamedTypeNode SerializeNamedType(
        INamedType namedType,
        Context context)
    {
        context.TypeNames.Add(namedType.Name);
        return new NamedTypeNode(null, new NameNode(namedType.Name));
    }

    private static IReadOnlyList<DirectiveNode> SerializeDirectives(
        IReadOnlyCollection<Directive> directives,
        Context context)
    {
        if (directives.Count == 0)
        {
            return Array.Empty<DirectiveNode>();
        }

        List<DirectiveNode>? directiveNodes = null;

        foreach (var directive in directives)
        {
            if (context.DirectiveNames.Contains(directive.Type.Name))
            {
                (directiveNodes ??= new()).Add(directive.AsSyntaxNode(true));
            }
        }

        if (directiveNodes is not null)
        {
            return directiveNodes;
        }

        return Array.Empty<DirectiveNode>();
    }

    private static StringValueNode? SerializeDescription(string? description)
        => description is { Length: > 0 }
            ? new StringValueNode(description)
            : null;

    private static bool IncludeType(INamedType type)
        => !IsBuiltInType(type) &&
           !IsApolloFederationType(type);

    private static bool IncludeField(IOutputField field)
        => !field.IsIntrospectionField &&
           !IsApolloFederationType(field.Type.NamedType());

    private static bool IsApolloFederationType(INamedType type)
        => type is EntityType or ServiceType ||
           type.Name.EqualsOrdinal(WellKnownTypeNames.Any) ||
           type.Name.EqualsOrdinal(WellKnownTypeNames.FieldSet);

    private static bool IsBuiltInType(INamedType type) =>
        IntrospectionTypes.IsIntrospectionType(type.Name) ||
        BuiltInTypes.IsBuiltInType(type.Name);

    private sealed class Context
    {
        // ReSharper disable once CollectionNeverQueried.Local
        public HashSet<string> TypeNames { get; } = new();
        public HashSet<string> DirectiveNames { get; } = new(_builtInDirectives);
    }
}
