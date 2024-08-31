using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal sealed class AsScalarDestructor(bool isMutable = AsScalarDestructor.DefaultIsMutableValue) : IPropertyDestructor, IEntityDestructor
{
    internal const bool DefaultIsMutableValue = false;
    
    public LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue, 
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        return new LogEventProperty(propertyName, CreateLogEventPropertyValue(propertyValue, propertyValueFactory));
    }

    public LogEventPropertyValue CreateLogEventPropertyValue(object? entity, ILogEventPropertyValueFactory propertyValueFactory)
    {
        var actualValue = isMutable ? entity?.ToString() : entity;
        return actualValue is null ? ScalarValue.Null : new ScalarValue(actualValue);
    }
}