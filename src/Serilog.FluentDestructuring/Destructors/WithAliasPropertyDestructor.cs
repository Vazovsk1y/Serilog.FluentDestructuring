using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal sealed class WithAliasPropertyDestructor(string propertyAlias) : IPropertyDestructor
{
    public LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue,
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        return new LogEventProperty(propertyAlias, propertyValueFactory.CreatePropertyValue(propertyValue, true));
    }
}