namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class ConditionalPropertyDestructuringModel
{
    public Guid? WithAliasIfNotNull { get; init; }
    
    public int? IgnoreIfNull { get; init; }

    public string? MaskIfCustomPredicate { get; init; }
}