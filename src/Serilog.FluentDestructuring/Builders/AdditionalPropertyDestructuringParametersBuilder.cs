using System.Reflection;

namespace Serilog.FluentDestructuring.Builders;

public abstract class AdditionalPropertyDestructuringParametersBuilder
{
    internal abstract (string? propertyAlias, Func<object, bool>? applyDestructuringPredicate) Build();
}

/// <summary>
/// Builder class to configure additional property destructuring parameters for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity whose property is being configured.</typeparam>
public sealed class AdditionalPropertyDestructuringParametersBuilder<TEntity> : ConditionalPropertyDestructuringApplyingBuilder<TEntity>
{
    private string? _propertyAlias;

    internal AdditionalPropertyDestructuringParametersBuilder(PropertyInfo targetPropertyInfo) : base(targetPropertyInfo) { }

    /// <summary>
    /// Specifies an alias for the target property when it is destructured.
    /// </summary>
    /// <param name="propertyAlias">The alias to be used for the property. Cannot be null, empty or whitespace.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="propertyAlias"/> is null, empty or consists only of white-space characters.</exception>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> WithAlias(string propertyAlias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyAlias);

        _propertyAlias = propertyAlias;
        return this;
    }
    
    internal override (string? propertyAlias, Func<object, bool>? applyDestructuringPredicate) Build()
    {
        return (_propertyAlias, ApplyDestructuringPredicate);
    }
}