namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class AdditionalPropertyDestructuringParametersModel
{
    public string? MaskWithCustomAlias { get; init; }
    
    public string? MaskWithCustomAliasAndApplyingPredicate { get; init; }
}