using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.Federation.HotChocolate;

/// <summary>
/// This attribute is used to mark types as an extended type
/// of a type that is defined by another service when
/// using apollo federation.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Interface)]
public sealed class ExtendServiceTypeAttribute : ObjectTypeDescriptorAttribute
{
    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectTypeDescriptor descriptor,
        Type type)
        => descriptor.ExtendServiceType();
}
