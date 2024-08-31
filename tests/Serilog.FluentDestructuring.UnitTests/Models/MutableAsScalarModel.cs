namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class MutableAsScalarModel
{
    public required int FirstProperty { get; set; }
    
    public required Guid SecondProperty { get; set; }
}