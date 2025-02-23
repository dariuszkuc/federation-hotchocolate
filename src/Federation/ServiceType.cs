using ApolloGraphQL.Federation.HotChocolate.Constants;
using ApolloGraphQL.Federation.HotChocolate.Properties;

namespace ApolloGraphQL.Federation.HotChocolate;

/// <summary>
/// A new object type called _Service must be created.
/// This type must have an sdl: String! field which exposes the SDL of the service's schema.
///
/// This SDL (schema definition language) is a printed version of the service's
/// schema including the annotations of federation directives. This SDL does not
/// include the additions of the federation spec.
/// </summary>
public class ServiceType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Service)
            .Description(FederationResources.ServiceType_Description)
            .Field(WellKnownFieldNames.Sdl)
            .Type<NonNullType<StringType>>()
            .Resolve(resolver => FederationSchemaPrinter.Print(resolver.Schema));
}
