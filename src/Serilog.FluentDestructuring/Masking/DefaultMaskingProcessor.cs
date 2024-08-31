namespace Serilog.FluentDestructuring.Masking;

internal sealed class DefaultMaskingProcessor(DefaultMaskingProcessorOptions options) : IMaskingProcessor
{
    public bool TryMask(string value, out string? maskedValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            maskedValue = null;
            return false;
        }

        maskedValue = new string(options.MaskCharacter, options.PreserveValueLength ? value.Length : (int)options.MaskLength);
        return true;
    }
}

/// <summary>
/// Options for configuring the behavior of a default masking processor.
/// </summary>
public sealed class DefaultMaskingProcessorOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the length of the original value should be preserved
    /// when applying the mask. If <c>true</c>, the masked value will have the same length as the original value,
    /// and the <see cref="MaskLength"/> property will be ignored.
    /// </summary>
    public bool PreserveValueLength { get; init; }

    /// <summary>
    /// Gets or sets the character used for masking. The default is an asterisk ('*').
    /// </summary>
    public char MaskCharacter { get; init; } = '*';

    /// <summary>
    /// Gets or sets the length of the mask to be applied. This is the number of <see cref="MaskCharacter"/>s
    /// that will be used to obfuscate the value. The default is 10.
    /// This property is ignored if <see cref="PreserveValueLength"/> is set to <c>true</c>.
    /// </summary>
    public uint MaskLength { get; init; } = 10;
}
