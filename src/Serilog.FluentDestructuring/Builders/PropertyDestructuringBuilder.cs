using System.Reflection;
using Serilog.FluentDestructuring.Destructors;
using Serilog.FluentDestructuring.Masking;

namespace Serilog.FluentDestructuring.Builders;

/// <summary>
/// Builder class for configuring the destructuring behavior of a specific property in an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity that contains the property.</typeparam>
public sealed class PropertyDestructuringBuilder<TEntity>
{
    private AdditionalPropertyDestructuringParametersBuilder? _additionalPropertyDestructuringParametersBuilder;
    private readonly PropertyInfo _targetPropertyInfo;
    private IPropertyDestructor _propertyDestructor = new NotSelectedPropertyDestructor();

    internal PropertyDestructuringBuilder(PropertyInfo targetPropertyInfo)
    {
        _targetPropertyInfo = targetPropertyInfo;
    }

    /// <summary>
    /// Sets an alias for the property when destructuring.
    /// </summary>
    /// <param name="propertyAlias">The alias to use for the property.</param>
    /// <returns>A builder to conditionally apply destructuring based on custom predicates.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="propertyAlias"/> is null, empty, or consists only of white-space characters.</exception>
    public ConditionalPropertyDestructuringApplyingBuilder<TEntity> WithAlias(string propertyAlias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyAlias);
        
        _propertyDestructor = new WithAliasPropertyDestructor(propertyAlias);
        var builder = new ConditionalPropertyDestructuringApplyingBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }
    
    /// <summary>
    /// Ignores the property during destructuring, meaning it will not be included in the log output.
    /// </summary>
    /// <returns>A builder to conditionally apply destructuring based on custom predicates.</returns>
    public ConditionalPropertyDestructuringApplyingBuilder<TEntity> Ignore()
    {
        _propertyDestructor = new IgnorePropertyDestructor();
        var builder = new ConditionalPropertyDestructuringApplyingBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }

    /// <summary>
    /// Configures the property to be logged as a scalar value, with an optional mutability setting.
    /// </summary>
    /// <param name="isMutable">Indicates whether the scalar value is mutable. Defaults to <c>false</c>.</param>
    /// <returns>A builder to further configure the destructuring behavior.</returns>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> AsScalar(bool isMutable = AsScalarDestructor.DefaultIsMutableValue)
    {
        _propertyDestructor = new AsScalarDestructor(isMutable);
        var builder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }
    
    /// <summary>
    /// Masks the property value during logging using the default masking processor.
    /// </summary>
    /// <returns>A builder to further configure the destructuring behavior.</returns>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> Mask()
    {
        _propertyDestructor = new MaskPropertyDestructor(new DefaultMaskingProcessor(new DefaultMaskingProcessorOptions()));
        var builder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }
    
    /// <summary>
    /// Masks the property value during logging using a default masking processor with specific options.
    /// </summary>
    /// <param name="options">The options to use for the masking processor.</param>
    /// <returns>A builder to further configure the destructuring behavior.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> Mask(DefaultMaskingProcessorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        _propertyDestructor = new MaskPropertyDestructor(new DefaultMaskingProcessor(options));
        var builder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }
    
    /// <summary>
    /// Masks the property value during logging using a custom masking processor.
    /// </summary>
    /// <param name="maskingProcessor">The custom masking processor to use.</param>
    /// <returns>A builder to further configure the destructuring behavior.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="maskingProcessor"/> is null.</exception>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> Mask(IMaskingProcessor maskingProcessor)
    {
        ArgumentNullException.ThrowIfNull(maskingProcessor);
        
        _propertyDestructor = new MaskPropertyDestructor(maskingProcessor);
        var builder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(_targetPropertyInfo);
        _additionalPropertyDestructuringParametersBuilder = builder;
        
        return builder;
    }
    
    internal SimplePropertyDestructuringConfiguration Build()
    {
        var (propertyAlias, applyDestructuringCondition) = _additionalPropertyDestructuringParametersBuilder?.Build() ?? (null, null);

        return new SimplePropertyDestructuringConfiguration(
            _propertyDestructor,
            string.IsNullOrWhiteSpace(propertyAlias) ? _targetPropertyInfo.Name : propertyAlias,
            applyDestructuringCondition);
    }
}