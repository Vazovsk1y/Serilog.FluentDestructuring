using System.Linq.Expressions;
using System.Reflection;
using Serilog.FluentDestructuring.Destructors;

namespace Serilog.FluentDestructuring.Builders;

public abstract class EntityDestructuringBuilder
{
    internal abstract EntityDestructuringConfiguration Build();
}

/// <summary>
/// Builder class for configuring how an entity is destructured for logging purposes.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being configured.</typeparam>
public sealed class EntityDestructuringBuilder<TEntity> : EntityDestructuringBuilder
{
    private readonly Dictionary<PropertyInfo, PropertyDestructuringBuilder<TEntity>> _simplePropertyBuilders = new();
    private readonly Dictionary<PropertyInfo, (EntityDestructuringBuilder, AdditionalPropertyDestructuringParametersBuilder)> _innerEntityBuilders = new();
    
    private IEntityDestructor? _entityDestructor;
    
    internal EntityDestructuringBuilder() { }

    /// <summary>
    /// Configures the entity to be logged as a scalar value.
    /// </summary>
    /// <param name="isMutable">Indicates whether the scalar value is mutable. Defaults to <c>false</c>.</param>
    public void AsScalar(bool isMutable = AsScalarDestructor.DefaultIsMutableValue)
    {
        _entityDestructor = new AsScalarDestructor(isMutable);
    }

    /// <summary>
    /// Configures how a specific property of the entity should be destructured.
    /// </summary>
    /// <param name="property">An expression that identifies the property to be configured.</param>
    /// <returns>A builder for further configuring the destructuring of the specified property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="property"/> expression is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a nested property is used in the expression.</exception>
    public PropertyDestructuringBuilder<TEntity> Property(Expression<Func<TEntity, object?>> property)
    {
        ArgumentNullException.ThrowIfNull(property);

        var propertyInfo = GetPropertyInfo(property);
        var result = new PropertyDestructuringBuilder<TEntity>(propertyInfo);

        _innerEntityBuilders.Remove(propertyInfo, out _);
        _simplePropertyBuilders[propertyInfo] = result;
        
        return result;
    }
    
    /// <summary>
    /// Configures how an inner entity property should be destructured with additional options.
    /// </summary>
    /// <typeparam name="TInnerEntity">The type of the inner entity.</typeparam>
    /// <param name="innerEntityProperty">An expression that identifies the inner entity property to be configured.</param>
    /// <param name="configureInnerEntityDestructuring">An action to configure the destructuring of the inner entity.</param>
    /// <returns>A builder for further configuring additional options for the inner entity property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="innerEntityProperty"/> or <paramref name="configureInnerEntityDestructuring"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a nested property is used in the expression.</exception>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> InnerEntity<TInnerEntity>(
        Expression<Func<TEntity, TInnerEntity?>> innerEntityProperty, 
        Action<EntityDestructuringBuilder<TInnerEntity>> configureInnerEntityDestructuring)
    {
        ArgumentNullException.ThrowIfNull(innerEntityProperty);
        ArgumentNullException.ThrowIfNull(configureInnerEntityDestructuring);

        var propertyInfo = GetPropertyInfo(innerEntityProperty);
        _simplePropertyBuilders.Remove(propertyInfo, out _);

        var innerEntityBuilder = new EntityDestructuringBuilder<TInnerEntity>();
        configureInnerEntityDestructuring.Invoke(innerEntityBuilder);

        var additionalPropertyDestructuringParametersBuilder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(propertyInfo);
        _innerEntityBuilders[propertyInfo] = (innerEntityBuilder, additionalPropertyDestructuringParametersBuilder);
        
        return additionalPropertyDestructuringParametersBuilder;
    }
    
    /// <summary>
    /// Configures how an inner entity property should be destructured using a predefined configuration.
    /// </summary>
    /// <typeparam name="TInnerEntity">The type of the inner entity.</typeparam>
    /// <param name="innerEntityProperty">An expression that identifies the inner entity property to be configured.</param>
    /// <param name="innerEntityConfiguration">The predefined configuration for destructuring the inner entity.</param>
    /// <returns>A builder for further configuring additional options for the inner entity property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="innerEntityProperty"/> or <paramref name="innerEntityConfiguration"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a nested property is used in the expression.</exception>
    public AdditionalPropertyDestructuringParametersBuilder<TEntity> InnerEntity<TInnerEntity>(
        Expression<Func<TEntity, TInnerEntity?>> innerEntityProperty,
        IEntityDestructuringConfiguration<TInnerEntity> innerEntityConfiguration)
    {
        ArgumentNullException.ThrowIfNull(innerEntityProperty);
        ArgumentNullException.ThrowIfNull(innerEntityConfiguration);
        
        var propertyInfo = GetPropertyInfo(innerEntityProperty);
        _simplePropertyBuilders.Remove(propertyInfo, out _);
        
        var innerEntityBuilder = new EntityDestructuringBuilder<TInnerEntity>();
        innerEntityConfiguration.Configure(innerEntityBuilder);
        
        var additionalPropertyDestructuringParametersBuilder = new AdditionalPropertyDestructuringParametersBuilder<TEntity>(propertyInfo);
        _innerEntityBuilders[propertyInfo] = (innerEntityBuilder, additionalPropertyDestructuringParametersBuilder);
        
        return additionalPropertyDestructuringParametersBuilder;
    }
    
    internal override EntityDestructuringConfiguration Build()
    {
        var propertyConfigurations = _simplePropertyBuilders
            .ToDictionary(a => a.Key, a => (PropertyDestructuringConfiguration)a.Value.Build());

        foreach (var kvp in _innerEntityBuilders)
        {
            var (propertyAlias, applyDestructuringPredicate) = kvp.Value.Item2.Build();
            propertyConfigurations[kvp.Key] = new InnerEntityDestructuringConfiguration(
                kvp.Value.Item1.Build(),
                string.IsNullOrWhiteSpace(propertyAlias) ? kvp.Key.Name : propertyAlias,
                applyDestructuringPredicate
            );
        }

        return new EntityDestructuringConfiguration(_entityDestructor, propertyConfigurations);
    }

    private static PropertyInfo GetPropertyInfo<T>(Expression<Func<TEntity, T?>> expression)
    {
        if (expression.Body is not MemberExpression body)
        {
            var uBody = (UnaryExpression)expression.Body;
            body = (MemberExpression)uBody.Operand;
        }

        var propertyInfo = body.Member as PropertyInfo;
        ArgumentNullException.ThrowIfNull(propertyInfo);

        if (body.Expression is MemberExpression)
        {
            throw new InvalidOperationException($"Nested properties are not allowed. Use only single-level properties or configure inner entity by calling '{nameof(InnerEntity)}'.");
        }

        return propertyInfo;
    }
}