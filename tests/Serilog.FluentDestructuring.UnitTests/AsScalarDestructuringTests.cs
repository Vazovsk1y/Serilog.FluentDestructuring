using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class AsScalarDestructuringTests
{
    [Fact]
    public void Immutable_Model_Should_Be_Logged_As_Scalar_When_This_Is_Configured()
    {
        var obj = new ImmutableAsScalarModel(6, Guid.NewGuid());
        var expected = new ScalarValue(obj);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var actual = (ScalarValue)evt.Properties[DelegateSink.ParamName];

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Mutable_Model_Should_Be_Logged_As_Scalar_When_This_Is_Configured()
    {
        var obj = new MutableAsScalarModel
        {
            FirstProperty = 6,
            SecondProperty = Guid.NewGuid(),
        };
        var expected = new ScalarValue(obj.ToString());
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var actual = (ScalarValue)evt.Properties[DelegateSink.ParamName];

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Immutable_Property_Should_Be_Logged_As_Scalar_When_This_Is_Configured()
    {
        var obj = new AsScalarPropertyModel
        {
            ImmutableAsScalarModel = new ImmutableAsScalarModel(7, Guid.NewGuid()),
            MutableAsScalarModel = new MutableAsScalarModel
            {
                FirstProperty = 9,
                SecondProperty = Guid.NewGuid(),
            }
        };
        var expected = new ScalarValue(obj.ImmutableAsScalarModel);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(AsScalarPropertyModel.ImmutableAsScalarModel)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Mutable_Property_Should_Be_Logged_As_Scalar_When_This_Is_Configured()
    {
        var obj = new AsScalarPropertyModel
        {
            ImmutableAsScalarModel = new ImmutableAsScalarModel(7, Guid.NewGuid()),
            MutableAsScalarModel = new MutableAsScalarModel
            {
                FirstProperty = 9,
                SecondProperty = Guid.NewGuid(),
            }
        };
        var expected = new ScalarValue(obj.MutableAsScalarModel.ToString());
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(AsScalarPropertyModel.MutableAsScalarModel)].Should().BeEquivalentTo(expected);
    }
}