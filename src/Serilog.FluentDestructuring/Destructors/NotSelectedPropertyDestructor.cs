using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal sealed class NotSelectedPropertyDestructor : IPropertyDestructor
{
    public LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue, 
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        const string value = "Property destructuring option has not been selected.";
        return new LogEventProperty(propertyName, propertyValueFactory.CreatePropertyValue(value, true));
    }
}