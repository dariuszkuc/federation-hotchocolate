using ApolloGraphQL.Federation.HotChocolate.Constants;
using HotChocolate.Language;
using HotChocolate.Types;
using Xunit;
using static HotChocolate.Language.Utf8GraphQLParser;

namespace ApolloGraphQL.Federation.HotChocolate;

public class FieldSetTypeTests
{
    [Fact]
    public void Ensure_Type_Name_Is_Correct()
    {
        // arrange
        // act
        var type = new FieldSetType();

        // assert
        Assert.Equal(WellKnownTypeNames.FieldSet, type.Name);
    }

    [Fact]
    public void Deserialize()
    {
        // arrange
        var type = new FieldSetType();
        const string serialized = "a b c d e(d: $b)";

        // act
        var selectionSet = type.Deserialize(serialized);

        // assert
        Assert.IsType<SelectionSetNode>(selectionSet);
    }

    [Fact]
    public void Deserialize_Invalid_Format()
    {
        // arrange
        var type = new FieldSetType();
        const string serialized = "1";

        // act
        void Action() => type.Deserialize(serialized);

        // assert
        Assert.Throws<SerializationException>(Action);
    }

    [Fact]
    public void TryDeserialize()
    {
        // arrange
        var type = new FieldSetType();
        const string serialized = "a b c d e(d: $b)";

        // act
        var success = type.TryDeserialize(serialized, out var selectionSet);

        // assert
        Assert.True(success);
        Assert.IsType<SelectionSetNode>(selectionSet);
    }

    [Fact]
    public void TryDeserialize_Null()
    {
        // arrange
        var type = new FieldSetType();

        // act
        var success = type.TryDeserialize(null, out var selectionSet);

        // assert
        Assert.True(success);
        Assert.Null(selectionSet);
    }

    [Fact]
    public void TryDeserialize_Invalid_Syntax()
    {
        // arrange
        var type = new FieldSetType();
        const string serialized = "1";

        // act
        var success = type.TryDeserialize(serialized, out var selectionSet);

        // assert
        Assert.False(success);
        Assert.Null(selectionSet);
    }

    [Fact]
    public void TryDeserialize_Invalid_Type()
    {
        // arrange
        var type = new FieldSetType();
        const int serialized = 1;

        // act
        var success = type.TryDeserialize(serialized, out var selectionSet);

        // assert
        Assert.False(success);
        Assert.Null(selectionSet);
    }

    [Fact]
    public void Serialize()
    {
        // arrange
        var type = new FieldSetType();
        var selectionSet = Syntax.ParseSelectionSet("{ a b c d e(d: $b) }");

        // act
        var serialized = type.Serialize(selectionSet);

        // assert
        Assert.Equal("a b c d e(d: $b)", serialized);
    }

    [Fact]
    public void Serialize_Invalid_Format()
    {
        // arrange
        var type = new FieldSetType();

        // act
        void Action() => type.Serialize(1);

        // assert
        Assert.Throws<SerializationException>(Action);
    }

    [Fact]
    public void TrySerialize()
    {
        // arrange
        var type = new FieldSetType();
        var selectionSet = Syntax.ParseSelectionSet("{ a b c d e(d: $b) }");

        // act
        var success = type.TrySerialize(selectionSet, out var serialized);

        // assert
        Assert.True(success);
        Assert.Equal("a b c d e(d: $b)", serialized);
    }

    [Fact]
    public void TrySerialize_Invalid_Format()
    {
        // arrange
        var type = new FieldSetType();

        // act
        var success = type.TrySerialize(1, out var serialized);

        // assert
        Assert.False(success);
        Assert.Null(serialized);
    }

    [Fact]
    public void ParseValue()
    {
        // arrange
        var type = new FieldSetType();
        var selectionSet = Syntax.ParseSelectionSet("{ a b c d e(d: $b) }");

        // act
        var valueSyntax = type.ParseValue(selectionSet);

        // assert
        Assert.Equal(
            "a b c d e(d: $b)",
            Assert.IsType<StringValueNode>(valueSyntax).Value);
    }

    [Fact]
    public void ParseValue_Null()
    {
        // arrange
        var type = new FieldSetType();

        // act
        var valueSyntax = type.ParseValue(null);

        // assert
        Assert.IsType<NullValueNode>(valueSyntax);
    }

    [Fact]
    public void ParseValue_InvalidValue()
    {
        // arrange
        var type = new FieldSetType();

        // act
        void Action() => type.ParseValue(1);

        // assert
        Assert.Throws<SerializationException>(Action);
    }
}
