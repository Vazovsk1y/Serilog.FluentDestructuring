using Serilog.Core;
using Serilog.Events;

namespace Serilog.FluentDestructuring.UnitTests.Infrastructure;

internal sealed class DelegateSink : ILogEventSink
{
    private readonly Action<LogEvent> _write;

    private DelegateSink(Action<LogEvent> write)
    {
        _write = write;
    }

    internal const string ParamName = "Obj";

    public void Emit(LogEvent logEvent) => _write(logEvent);

    public static LogEvent Execute<T>(object? obj, string messageTemplate = $"Logged object - {{@{ParamName}}}.", Action<FluentDestructuringPolicyOptions>? configureOptions = null) 
        where T : FluentDestructuringPolicy, new()
    {
        LogEvent evt = null!;

        var config = new LoggerConfiguration();
        config = configureOptions is null ? config.Destructure.WithFluentDestructuringPolicy<T>() : config.Destructure.WithFluentDestructuringPolicy<T>(configureOptions);
        config = config.WriteTo.Sink(new DelegateSink(e => evt = e));

        var log = config.CreateLogger();
        log.Information(messageTemplate, obj);

        return evt;
    }
}