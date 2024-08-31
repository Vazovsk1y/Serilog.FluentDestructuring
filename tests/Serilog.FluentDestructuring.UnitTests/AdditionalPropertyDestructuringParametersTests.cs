using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class AdditionalPropertyDestructuringParametersTests
{
    [Fact]
    public void Property_Should_Be_Masked_With_Custom_Alias_When_This_Is_Configured()
    {
        var obj = new AdditionalPropertyDestructuringParametersModel()
        {
            MaskWithCustomAlias = "string"
        };
        var expected = new ScalarValue("**********");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(AdditionalPropertyDestructuringParametersModel.MaskWithCustomAlias));
        properties["mask_with_custom_alias"].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Property_Should_Be_Masked_With_Custom_Alias_When_This_Is_Configured_And_Applying_Predicate_Matches()
    {
        var obj = new AdditionalPropertyDestructuringParametersModel()
        {
            MaskWithCustomAliasAndApplyingPredicate = "string",
            MaskWithCustomAlias = "value"
        };
        var expected = new ScalarValue("**********");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(AdditionalPropertyDestructuringParametersModel.MaskWithCustomAliasAndApplyingPredicate));
        properties["mask_with_custom_alias_and_applying_predicate"].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Property_Should_Not_Be_Masked_With_Custom_Alias_When_This_Is_Configured_And_Applying_Predicate_Does_Not_Match()
    {
        var obj = new AdditionalPropertyDestructuringParametersModel()
        {
            MaskWithCustomAliasAndApplyingPredicate = "string",
            MaskWithCustomAlias = "not value"
        };
        var expected = new ScalarValue(obj.MaskWithCustomAliasAndApplyingPredicate);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey("mask_with_custom_alias_and_applying_predicate");
        properties[nameof(AdditionalPropertyDestructuringParametersModel.MaskWithCustomAliasAndApplyingPredicate)].Should().BeEquivalentTo(expected);
    }
}