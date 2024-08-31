using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class FluentDestructuringPolicyTests
{
    [Fact]
    public void Null_Properties_Should_Be_Ignored_When_This_Is_Configured_With_Global_Options()
    {
        var obj = new FluentDestructuringPolicyModel()
        {
            NullProperty2 = null,
            NullProperty = null,
            StringProperty = "string"
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj, configureOptions: e => e.IgnoreNullProperties = true);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(FluentDestructuringPolicyModel.NullProperty));
        properties.Should().NotContainKey(nameof(FluentDestructuringPolicyModel.NullProperty2));
    }
    
    [Fact]
    public void Type_Tag_Should_Be_Null_When_This_Is_Configured_With_Global_Options()
    {
        var obj = new FluentDestructuringPolicyModel()
        {
            NullProperty2 = null,
            NullProperty = null,
            StringProperty = "string"
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj, configureOptions: e => e.ExcludeTypeTag = true);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];

        sv.TypeTag.Should().BeNull();
    }

    [Fact]
    public void Type_Tag_Should_Be_Null_When_This_Is_Configured_With_Global_Options_And_No_Configuration_For_Entity_Provided()
    {
        var obj = new WithoutConfigurationModel()
        {
            NullableDecimal = 33,
            InnerModel = new WithoutConfigurationInnerModel()
            {
                NullableInt32 = 5,
            }
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj, configureOptions: e => e.ExcludeTypeTag = true);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        sv.TypeTag.Should().BeNull();
        properties[nameof(WithoutConfigurationModel.InnerModel)].As<StructureValue>().TypeTag.Should().BeNull();
    }
    
    [Fact]
    public void Null_Properties_Should_Be_Ignored_When_This_Is_Configured_With_Global_Options_And_No_Configuration_For_Entity_Provided()
    {
        var obj = new WithoutConfigurationModel()
        {
            NullableDecimal = null,
            InnerModel = new WithoutConfigurationInnerModel()
            {
                NullableInt32 = null,
            }
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj, configureOptions: e => e.IgnoreNullProperties = true);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(WithoutConfigurationModel.NullableDecimal));
        properties[nameof(WithoutConfigurationModel.InnerModel)].As<StructureValue>()
            .Properties.ToDictionary(e => e.Name, e => e.Value).Should()
            .NotContainKey(nameof(WithoutConfigurationInnerModel.NullableInt32));
    }
}