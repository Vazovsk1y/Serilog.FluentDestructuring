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

    #region Issue 2
    
    // https://github.com/Vazovsk1y/Serilog.FluentDestructuring/issues/2

    [Fact]
    public void IEnumerable_Property_Should_Be_Logged_As_Sequence_Value_WHEN_No_Custom_Rule_Configured()
    {
        var obj = new FluentDestructuringPolicyModel
        {
            IEnumerable = Enumerable.Range(0, 3).Select(e => new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = $"property{e}" }),
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(FluentDestructuringPolicyModel.IEnumerable)].Should().BeOfType<SequenceValue>();
    }
    
    [Fact]
    public void IEnumerable_Should_Be_Logged_As_Sequence_Value_WHEN_No_Custom_Rule_Configured()
    {
        var obj = Enumerable.Range(0, 3).Select(e => new InnerEntityPropertyModel() { Id = Guid.NewGuid(), Property = $"property{e}" });

        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var value = evt.Properties[DelegateSink.ParamName];

        value.Should().BeOfType<SequenceValue>();
    }
    
    [Fact]
    public void Dictionary_Property_Should_Be_Logged_As_Dictionary_Value_WHEN_No_Custom_Rule_Configured()
    {
        var obj = new FluentDestructuringPolicyModel()
        {
            Dictionary = new Dictionary<Guid, InnerEntityPropertyModel>
            {
                { Guid.NewGuid(), new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = "property1" } },
                { Guid.NewGuid(), new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = "property2" } }
            },
        };
    
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(FluentDestructuringPolicyModel.Dictionary)].Should().BeOfType<DictionaryValue>();
    }

    [Fact]
    public void Dictionary_Should_Be_Logged_As_Dictionary_Value_WHEN_No_Custom_Rule_Configured()
    {
        var obj = new Dictionary<Guid, InnerEntityPropertyModel>
        {
            { Guid.NewGuid(), new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = "property1" } },
            { Guid.NewGuid(), new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = "property2" } }
        };
    
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var value = evt.Properties[DelegateSink.ParamName];

        value.Should().BeOfType<DictionaryValue>();
    }

    [Fact]
    public void IEnumerable_Property_Should_Be_Logged_As_Configured_WHEN_That_Provided()
    {
        var obj = new FluentDestructuringPolicyModel
        {
            IEnumerableAsScalar = Enumerable.Range(0, 3).Select(e => new InnerEntityPropertyModel { Id = Guid.NewGuid(), Property = $"property{e}" }),
        };
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(FluentDestructuringPolicyModel.IEnumerableAsScalar)].Should().BeOfType<ScalarValue>();
    }
    
    #endregion
}