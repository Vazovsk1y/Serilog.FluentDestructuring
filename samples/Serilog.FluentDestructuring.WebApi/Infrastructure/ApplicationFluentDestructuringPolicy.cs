using System.Reflection;
using Serilog.FluentDestructuring.Builders;

namespace Serilog.FluentDestructuring.WebApi.Infrastructure;

public class ApplicationFluentDestructuringPolicy : FluentDestructuringPolicy
{
    protected override void Configure(FluentDestructuringBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}