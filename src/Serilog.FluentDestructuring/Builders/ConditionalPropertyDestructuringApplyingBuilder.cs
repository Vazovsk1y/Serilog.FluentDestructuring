using System.Linq.Expressions;
using System.Reflection;

namespace Serilog.FluentDestructuring.Builders;

/// <summary>
/// Builder class for conditionally applying destructuring rules to a specific property of an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity containing the property being configured.</typeparam>
public class ConditionalPropertyDestructuringApplyingBuilder<TEntity> : AdditionalPropertyDestructuringParametersBuilder
{
    private readonly PropertyInfo _targetPropertyInfo;
    protected Func<object, bool>? ApplyDestructuringPredicate;

    internal ConditionalPropertyDestructuringApplyingBuilder(PropertyInfo targetPropertyInfo)
    {
        _targetPropertyInfo = targetPropertyInfo;
    }

    /// <summary>
    /// Specifies a predicate to determine when the destructuring should be applied.
    /// </summary>
    /// <param name="applyDestructuringPredicate">A predicate that evaluates to true when destructuring should be applied.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="applyDestructuringPredicate"/> is null.</exception>
    public void ApplyWhen(Func<TEntity, bool> applyDestructuringPredicate)
    {
        ArgumentNullException.ThrowIfNull(applyDestructuringPredicate);

        ApplyDestructuringPredicate = obj => applyDestructuringPredicate((TEntity)obj);
    }

    /// <summary>
    /// Applies destructuring when the target property is null.
    /// </summary>
    public void ApplyWhenNull()
    {
        ApplyDestructuringPredicate = BuildPredicate(_targetPropertyInfo, ExpressionType.Equal, null);
    }

    /// <summary>
    /// Applies destructuring when the target property is not null.
    /// </summary>
    public void ApplyWhenNotNull()
    {
        ApplyDestructuringPredicate = BuildPredicate(_targetPropertyInfo, ExpressionType.NotEqual, null);
    }

    internal override (string? propertyAlias, Func<object, bool>? applyDestructuringPredicate) Build()
    {
        return (null, ApplyDestructuringPredicate);
    }

    private static Func<object, bool> BuildPredicate(PropertyInfo propertyInfo, ExpressionType expressionType, object? value)
    {
        var objParameter = Expression.Parameter(typeof(object), "obj");
        var castedObj = Expression.Convert(objParameter, typeof(TEntity));
        var propertyAccess = Expression.Property(castedObj, propertyInfo);
        var comparison = Expression.MakeBinary(expressionType, propertyAccess, Expression.Constant(value));
        var lambda = Expression.Lambda<Func<object, bool>>(comparison, objParameter).Compile();

        return lambda;
    }
}
