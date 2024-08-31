using Serilog.Configuration;

namespace Serilog.FluentDestructuring;

public static class LoggerDestructuringConfigurationExtensions
{
    /// <summary>
    /// Configures the logger to use a custom fluent destructuring policy.
    /// </summary>
    /// <typeparam name="TDestructuringPolicy">The type of the <see cref="FluentDestructuringPolicy"/> to use.</typeparam>
    /// <param name="configuration">The <see cref="LoggerDestructuringConfiguration"/> to apply the policy to.</param>
    /// <returns>A <see cref="LoggerConfiguration"/> object allowing further configuration of the logger.</returns>
    /// <remarks>
    /// This method adds the specified <typeparamref name="TDestructuringPolicy"/> destructuring policy to the logger configuration, enabling
    /// fluent and customizable destructuring of complex objects during logging.
    /// </remarks>
    public static LoggerConfiguration WithFluentDestructuringPolicy<TDestructuringPolicy>(this LoggerDestructuringConfiguration configuration) 
        where TDestructuringPolicy : FluentDestructuringPolicy, new()
    {
        return configuration.With<TDestructuringPolicy>();
    }

    /// <summary>
    /// Configures the logger to use a custom fluent destructuring policy with additional options.
    /// </summary>
    /// <typeparam name="TDestructuringPolicy">The type of the <see cref="FluentDestructuringPolicy"/> to use.</typeparam>
    /// <param name="configuration">The <see cref="LoggerDestructuringConfiguration"/> to apply the policy to.</param>
    /// <param name="configureFluentDestructuringPolicyOptions">An action to configure additional options for the policy.</param>
    /// <returns>A <see cref="LoggerConfiguration"/> object allowing further configuration of the logger.</returns>
    /// <remarks>
    /// This method adds the specified <typeparamref name="TDestructuringPolicy"/> destructuring policy to the logger configuration and
    /// allows customization of the policy through the <paramref name="configureFluentDestructuringPolicyOptions"/> action.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="configureFluentDestructuringPolicyOptions"/> is <c>null</c>.
    /// </exception>
    public static LoggerConfiguration WithFluentDestructuringPolicy<TDestructuringPolicy>(this LoggerDestructuringConfiguration configuration, Action<FluentDestructuringPolicyOptions> configureFluentDestructuringPolicyOptions) 
        where TDestructuringPolicy : FluentDestructuringPolicy, new()
    {
        ArgumentNullException.ThrowIfNull(configureFluentDestructuringPolicyOptions);

        var policy = new TDestructuringPolicy();
        policy.ConfigureOptions(configureFluentDestructuringPolicyOptions);

        return configuration.With(policy);
    }
}