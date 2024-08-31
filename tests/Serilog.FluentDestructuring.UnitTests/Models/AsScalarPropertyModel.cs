namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class AsScalarPropertyModel
{
    public required ImmutableAsScalarModel ImmutableAsScalarModel { get; init; }
    
    public required MutableAsScalarModel MutableAsScalarModel { get; init; }
}