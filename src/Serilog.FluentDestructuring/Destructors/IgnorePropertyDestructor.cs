using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal sealed class IgnorePropertyDestructor : IPropertyDestructor
{
    public LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue, 
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        return null;
    }
}