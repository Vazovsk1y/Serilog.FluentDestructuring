using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class ConditionalPropertyDestructuringTests
{
    [Fact]
    public void Should_Apply_Property_Destructuring_When_Null_Predicate_Matches()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            IgnoreIfNull = null,
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(ConditionalPropertyDestructuringModel.IgnoreIfNull));
    }
    
    [Fact]
    public void Should_Not_Apply_Property_Destructuring_When_Null_Predicate_Does_Not_Match()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            IgnoreIfNull = 6,
        };
        var expected = new ScalarValue(obj.IgnoreIfNull);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(ConditionalPropertyDestructuringModel.IgnoreIfNull)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Should_Apply_Property_Destructuring_When_NotNull_Predicate_Matches()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            WithAliasIfNotNull = Guid.NewGuid(),
        };
        var expected = new ScalarValue(obj.WithAliasIfNotNull);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties["with_alias_if_not_null"].Should().BeEquivalentTo(expected);
        properties.Should().NotContainKey(nameof(ConditionalPropertyDestructuringModel.WithAliasIfNotNull));
    }
    
    [Fact]
    public void Should_Not_Apply_Property_Destructuring_When_NotNull_Predicate_Does_Not_Match()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            WithAliasIfNotNull = null,
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey("with_alias_if_not_null");
        properties[nameof(ConditionalPropertyDestructuringModel.WithAliasIfNotNull)].Should().BeEquivalentTo(ScalarValue.Null);
    }
    
    [Fact]
    public void Should_Apply_Property_Destructuring_When_Custom_Predicate_Matches()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            MaskIfCustomPredicate = "string",
            WithAliasIfNotNull = Guid.Empty,
            IgnoreIfNull = 11,
        };
        var expected = new ScalarValue("**********");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(ConditionalPropertyDestructuringModel.MaskIfCustomPredicate)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Should_Not_Apply_Property_Destructuring_When_Custom_Predicate_Does_Not_Match()
    {
        var obj = new ConditionalPropertyDestructuringModel
        {
            MaskIfCustomPredicate = "string",
            WithAliasIfNotNull = Guid.NewGuid(),
            IgnoreIfNull = 9,
        };
        var expected = new ScalarValue(obj.MaskIfCustomPredicate);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(ConditionalPropertyDestructuringModel.MaskIfCustomPredicate)].Should().BeEquivalentTo(expected);
    }
}