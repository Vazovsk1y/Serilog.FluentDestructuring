using Serilog.Core;
using Serilog.Events;
using Serilog.FluentDestructuring.Masking;

namespace Serilog.FluentDestructuring.Destructors;

internal sealed class MaskPropertyDestructor(IMaskingProcessor maskingProcessor) : IPropertyDestructor
{
    public LogEventProperty? CreateLogEventProperty(
        string propertyName, 
        object? propertyValue, 
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        switch (propertyValue)
        {
            case null:
               return new LogEventProperty(propertyName, ScalarValue.Null);
            case string stringValue:
            {
                var maskingResult = maskingProcessor.TryMask(stringValue, out var maskedValue);
                return new LogEventProperty(propertyName, new ScalarValue(maskingResult ? maskedValue : stringValue));
            }
            case IEnumerable<string> strings:
            {
                var scalars = new List<ScalarValue>();
                foreach (var str in strings)
                {
                    var maskingResult = maskingProcessor.TryMask(str, out var maskedValue);
                    scalars.Add(new ScalarValue(maskingResult ? maskedValue : str));
                }

                return new LogEventProperty(propertyName, new SequenceValue(scalars));
            }
            default:
            {
                return new LogEventProperty(propertyName, propertyValueFactory.CreatePropertyValue(propertyValue, true));
            }
        }
    }
}