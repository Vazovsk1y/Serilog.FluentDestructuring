using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal interface IEntityDestructor
{
    LogEventPropertyValue CreateLogEventPropertyValue(object? entity, ILogEventPropertyValueFactory propertyValueFactory);
}