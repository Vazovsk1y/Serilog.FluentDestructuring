namespace Serilog.FluentDestructuring;

/// <summary>
/// Provides configuration options for the <see cref="FluentDestructuringPolicy"/>.
/// </summary>
public sealed class FluentDestructuringPolicyOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the type tag should be excluded from the destructured output.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, the type name of the object being destructured will not be included in the output.
    /// </remarks>
    public bool ExcludeTypeTag { get; set; }
        
    /// <summary>
    /// Gets or sets a value indicating whether properties with null values should be ignored during destructuring.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, properties with null values will not be included in the destructured output.
    /// </remarks>
    public bool IgnoreNullProperties { get; set; }
}