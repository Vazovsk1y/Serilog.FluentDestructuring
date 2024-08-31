using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class IgnorePropertyDestructuringTests
{
    [Fact]
    public void Property_Should_Be_Ignored_When_This_Is_Configured()
    {
        var obj = new IgnorePropertyModel
        {
            Ignore = 5,
        };

        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties.Should().NotContainKey(nameof(IgnorePropertyModel.Ignore));
    }
}