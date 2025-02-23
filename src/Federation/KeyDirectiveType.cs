using ApolloGraphQL.Federation.HotChocolate.Constants;
using ApolloGraphQL.Federation.HotChocolate.Properties;

namespace ApolloGraphQL.Federation.HotChocolate;

/// <summary>
/// The @key directive is used to indicate a combination of fields that
/// can be used to uniquely identify and fetch an object or interface.
/// <example>
/// type Product @key(fields: "upc") {
///   upc: UPC!
///   name: String
/// }
/// </example>
/// </summary>
public sealed class KeyDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Key)
            .Description(FederationResources.KeyDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.Interface)
            .Repeatable()
            .FieldsArgument();
}
