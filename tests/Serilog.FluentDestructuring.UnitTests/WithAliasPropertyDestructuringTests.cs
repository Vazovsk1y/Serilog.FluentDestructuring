using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class WithAliasPropertyDestructuringTests
{
    [Fact]
    public void Property_Should_Be_Logged_With_Custom_Alias_When_This_Is_Configured()
    {
        var obj = new WithAliasPropertyModel
        {
            SimpleProperty = 7,
        };
        var expected = new ScalarValue(obj.SimpleProperty);

        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(WithAliasPropertyModel.SimpleProperty));
        properties["simple_property"].Should().BeEquivalentTo(expected);
    }
}