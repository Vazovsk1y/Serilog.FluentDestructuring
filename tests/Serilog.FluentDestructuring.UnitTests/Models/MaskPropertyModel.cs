namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class MaskPropertyModel
{
    public string? String { get; init; }
    
    public string? StringPreserveLength { get; init; }
    
    public string? StringWithPreserveLengthAndCustomMaskCharacter { get; init; }
    
    public string? StringWithCustomMaskCharacter { get; init; }
    
    public string? StringWithCustomMaskCharacterAndCustomMaskLength { get; init; }
    
    public string? StringCustomMaskingProcessor { get; init; }
    
    public IEnumerable<string>? StringIEnumerable { get; init; }
    
    public int? NotString { get; init; }
}