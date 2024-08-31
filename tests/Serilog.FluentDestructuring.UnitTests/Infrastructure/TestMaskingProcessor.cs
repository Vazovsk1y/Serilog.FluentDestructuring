using Serilog.FluentDestructuring.Masking;

namespace Serilog.FluentDestructuring.UnitTests.Infrastructure;

internal sealed class TestMaskingProcessor : IMaskingProcessor
{
    public const string MaskedValue = "Mask";
    public bool TryMask(string value, out string? maskedValue)
    {
        maskedValue = MaskedValue;
        return true;
    }
}