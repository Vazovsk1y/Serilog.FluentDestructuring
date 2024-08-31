namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class InnerEntityPropertyModel
{
    public required Guid Id { get; init; }
    
    public required string Property { get; init; }
}