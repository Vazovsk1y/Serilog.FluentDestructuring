namespace Serilog.FluentDestructuring.Masking;

/// <summary>
/// Defines a contract for processing and applying masking to string values.
/// </summary>
public interface IMaskingProcessor
{
    /// <summary>
    /// Attempts to mask the specified string value.
    /// </summary>
    /// <param name="value">The string value to be masked.</param>
    /// <param name="maskedValue">
    /// When this method returns <c>true</c>, contains the masked version of the specified value, otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value was successfully masked; otherwise, <c>false</c>.
    /// </returns>
    bool TryMask(string value, out string? maskedValue);
}
