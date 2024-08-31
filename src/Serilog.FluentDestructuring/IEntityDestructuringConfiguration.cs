using Serilog.FluentDestructuring.Builders;

namespace Serilog.FluentDestructuring;

/// <summary>
/// Provides a contract for configuring the destructuring process for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be configured for destructuring.</typeparam>
public interface IEntityDestructuringConfiguration<TEntity>
{
    /// <summary>
    /// Configures the destructuring process for the specified entity type.
    /// </summary>
    /// <param name="builder">The builder used to configure the destructuring of the entity.</param>
    /// <remarks>
    /// Implement this method to define how the properties of <typeparamref name="TEntity"/> should be destructured.
    /// </remarks>
    void Configure(EntityDestructuringBuilder<TEntity> builder);
}