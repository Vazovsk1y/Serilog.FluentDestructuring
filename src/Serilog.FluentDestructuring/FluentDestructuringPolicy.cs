using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.FluentDestructuring.Builders;
using Serilog.FluentDestructuring.Destructors;

namespace Serilog.FluentDestructuring;

/// <summary>
/// Represents an abstract base class for implementing custom destructuring policies using fluent configuration. 
/// This class handles the process of destructuring complex objects into loggable properties based on configured rules.
/// </summary>
public abstract class FluentDestructuringPolicy : IDestructuringPolicy
{
    private readonly IReadOnlyDictionary<Type, EntityDestructuringConfiguration> _configurations;
    private readonly FluentDestructuringPolicyOptions _options;
    
    protected FluentDestructuringPolicy()
    {
        _options = new FluentDestructuringPolicyOptions();
        var builder = new FluentDestructuringBuilder();
        ConfigureCore(builder);
        _configurations = builder.Build();
    }
    
    public bool TryDestructure(object entity, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var entityType = entity.GetType();
        if (!_configurations.TryGetValue(entityType, out var entityConfiguration))
        {
            result = CreateLogEventValueDefault(entity, entityType, propertyValueFactory);
            return true;
        }

        result = CreateLogEventPropertyValue(entity, entityConfiguration, propertyValueFactory);
        return true;
    }

    private StructureValue CreateLogEventValueDefault(
        object entity,
        Type entityType,
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        var logEventProperties = new List<LogEventProperty>();

        foreach (var propertyInfo in GetPropertiesRecursive(entityType))
        {
            var propertyValue = GetPropertyValue(propertyInfo, entity);
            if (propertyValue is null && _options.IgnoreNullProperties)
            {
                continue;
            }
            
            logEventProperties.Add(new LogEventProperty(propertyInfo.Name, propertyValueFactory.CreatePropertyValue(propertyValue, true)));
        }

        return new StructureValue(logEventProperties, _options.ExcludeTypeTag ? null : entityType.Name);
    }
    
    private LogEventPropertyValue CreateLogEventPropertyValue(
        object? entity,
        EntityDestructuringConfiguration entityConfiguration,
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        if (entityConfiguration.EntityDestructor is not null)
        {
            return entityConfiguration.EntityDestructor.CreateLogEventPropertyValue(entity, propertyValueFactory);
        }
        
        if (entity is null)
        {
            return ScalarValue.Null;
        }
        
        var entityType = entity.GetType();
        var logEventProperties = new List<LogEventProperty>();

        foreach (var propertyInfo in GetPropertiesRecursive(entityType))
        {
            var propertyValue = GetPropertyValue(propertyInfo, entity);
            if (propertyValue is null && _options.IgnoreNullProperties)
            {
                continue;
            }
            
            if (entityConfiguration.PropertyDestructuringConfigurations.TryGetValue(propertyInfo, out var propertyConfig))
            {
                var logEventProperty = propertyConfig switch
                {
                    SimplePropertyDestructuringConfiguration simplePropertyConfig => HandleSimpleProperty(entity, propertyValue, propertyInfo, simplePropertyConfig, propertyValueFactory),
                    InnerEntityDestructuringConfiguration innerEntityConfig => innerEntityConfig.ApplyDestructuringPredicate is not null && !innerEntityConfig.ApplyDestructuringPredicate.Invoke(entity) ?
                        new LogEventProperty(propertyInfo.Name, propertyValueFactory.CreatePropertyValue(propertyValue, true))
                        :
                        new LogEventProperty(innerEntityConfig.PropertyAlias, CreateLogEventPropertyValue(propertyValue, innerEntityConfig.EntityConfiguration, propertyValueFactory)),
                    _ => throw new KeyNotFoundException(),
                };

                if (logEventProperty is not null)
                {
                    logEventProperties.Add(logEventProperty);
                }
            }
            else
            {
                logEventProperties.Add(new LogEventProperty(propertyInfo.Name, propertyValueFactory.CreatePropertyValue(propertyValue, true)));
            }
        }

        return new StructureValue(logEventProperties, _options.ExcludeTypeTag ? null : entityType.Name);
    }
    
    private static object? GetPropertyValue(PropertyInfo propertyInfo, object instance)
    {
        object? propertyValue;
        try
        {
            propertyValue = Compile(propertyInfo).Invoke(instance);
        }
        catch (Exception ex)
        {
            SelfLog.WriteLine("The property accessor {0} threw exception {1}.", propertyInfo, ex);
            propertyValue = $"The property accessor threw an exception: '{ex.GetType().Name}'.";
        }

        return propertyValue;
        
        static Func<object, object?> Compile(PropertyInfo property)
        {
            var instanceParamExpr = Expression.Parameter(typeof(object), "instance");
            ArgumentNullException.ThrowIfNull(property.DeclaringType);
            
            var instanceExpr = Expression.Convert(instanceParamExpr, property.DeclaringType);
            var propertyExpr = Expression.Property(instanceExpr, property);
            var convertedPropExpr = Expression.Convert(propertyExpr, typeof(object));
            return Expression.Lambda<Func<object, object?>>(convertedPropExpr, instanceParamExpr).Compile();
        }
    }

    private static LogEventProperty? HandleSimpleProperty(
        object instance,
        object? propertyValue,
        PropertyInfo propertyInfo,
        SimplePropertyDestructuringConfiguration propertyConfig, 
        ILogEventPropertyValueFactory propertyValueFactory)
    {
        if (propertyConfig.ApplyDestructuringPredicate is not null && !propertyConfig.ApplyDestructuringPredicate.Invoke(instance))
        {
            return new LogEventProperty(propertyInfo.Name, propertyValueFactory.CreatePropertyValue(propertyValue, true));
        }

        var result = propertyConfig.PropertyDestructor.CreateLogEventProperty(propertyConfig.PropertyAlias, propertyValue, propertyValueFactory);
        if (result is not null)
        {
            return result;
        }

        if (propertyConfig.PropertyDestructor is not IgnorePropertyDestructor)
        {
            result = new LogEventProperty(propertyConfig.PropertyAlias, propertyValueFactory.CreatePropertyValue(propertyValue, true));
        }
        
        return result;
    }

    private static IEnumerable<PropertyInfo> GetPropertiesRecursive(Type type)
    {
        var history = new HashSet<string>();

        while (true)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.GetMethod != null && p.CanRead && p.GetMethod.IsPublic && p.GetIndexParameters().Length == 0 && !history.Contains(p.Name));

            foreach (var propertyInfo in properties)
            {
                history.Add(propertyInfo.Name);
                yield return propertyInfo;
            }

            if (type.BaseType is null || type.BaseType == typeof(object))
            {
                break;
            }
            
            type = type.BaseType;
        }
    }

    internal void ConfigureOptions(Action<FluentDestructuringPolicyOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        configureOptions.Invoke(_options);
    }

    private void ConfigureCore(FluentDestructuringBuilder builder) => Configure(builder);

    /// <summary>
    /// Configures the builder with custom entity destructuring rules. 
    /// Implement this method to specify how entities should be destructured.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    protected abstract void Configure(FluentDestructuringBuilder builder);
}