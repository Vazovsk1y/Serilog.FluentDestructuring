namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class WithoutConfigurationModel
{
    public decimal? NullableDecimal { get; init; }
    
    public WithoutConfigurationInnerModel? InnerModel { get; init; }
}

internal class WithoutConfigurationInnerModel
{
    public int? NullableInt32 { get; init; }
}