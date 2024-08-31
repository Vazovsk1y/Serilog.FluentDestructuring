using System.Reflection;

namespace Serilog.FluentDestructuring.Builders;

/// <summary>
/// Main builder class for configuring the destructuring behavior of entities for logging.
/// </summary>
public sealed class FluentDestructuringBuilder
{
    private readonly Dictionary<Type, EntityDestructuringBuilder> _entityBuilders = new();
    
    internal FluentDestructuringBuilder() { }

    /// <summary>
    /// Configures the destructuring for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to configure.</typeparam>
    /// <param name="configureEntityDestructuring">An action to configure the destructuring of the entity.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configureEntityDestructuring"/> is null.</exception>
    public FluentDestructuringBuilder Entity<TEntity>(Action<EntityDestructuringBuilder<TEntity>> configureEntityDestructuring)
    {
        ArgumentNullException.ThrowIfNull(configureEntityDestructuring);
        
        var builder = new EntityDestructuringBuilder<TEntity>();
        configureEntityDestructuring.Invoke(builder);
        _entityBuilders[typeof(TEntity)] = builder;

        return this;
    }

    /// <summary>
    /// Applies a predefined configuration for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to configure.</typeparam>
    /// <param name="configuration">The predefined configuration to apply.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public FluentDestructuringBuilder ApplyConfiguration<TEntity>(IEntityDestructuringConfiguration<TEntity> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        var builder = new EntityDestructuringBuilder<TEntity>();
        configuration.Configure(builder);
        _entityBuilders[typeof(TEntity)] = builder;
        
        return this;
    }

    /// <summary>
    /// Applies all entity destructuring configurations found in a specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for entity destructuring configurations.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a configuration type does not have a public parameterless constructor.</exception>
    public FluentDestructuringBuilder ApplyConfigurationsFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        
        var applyEntityConfigurationMethod = typeof(FluentDestructuringBuilder)
            .GetMethods()
            .Single(
                e => e is { Name: nameof(ApplyConfiguration), ContainsGenericParameters: true }
                     && e.GetParameters().SingleOrDefault()?.ParameterType.GetGenericTypeDefinition()
                     == typeof(IEntityDestructuringConfiguration<>));
        
        var configurationTypes = assembly
            .GetTypes()
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityDestructuringConfiguration<>)));
        
        foreach (var configType in configurationTypes)
        {
            var ctor = configType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);

            if (ctor is null)
            {
                throw new InvalidOperationException($"It is necessary to provide a public parameterless constructor for type '{configType.Name}'.");
            }
            
            var entityType = configType.GetInterfaces().Single(e => e.GetGenericTypeDefinition() == typeof(IEntityDestructuringConfiguration<>)).GenericTypeArguments[0];
            var target = applyEntityConfigurationMethod.MakeGenericMethod(entityType);
            target.Invoke(this, [ Activator.CreateInstance(configType) ]);
        }

        return this;
    }

    internal IReadOnlyDictionary<Type, EntityDestructuringConfiguration> Build()
    {
        return _entityBuilders.ToDictionary(e => e.Key, e => e.Value.Build());
    }
}
