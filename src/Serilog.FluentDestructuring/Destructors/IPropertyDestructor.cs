using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.Destructors;

internal interface IPropertyDestructor
{
    LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue, 
        ILogEventPropertyValueFactory propertyValueFactory);
}


