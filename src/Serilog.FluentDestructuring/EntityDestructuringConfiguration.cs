using System.Reflection;
using Serilog.FluentDestructuring.Destructors;

namespace Serilog.FluentDestructuring;

internal record EntityDestructuringConfiguration(
    IEntityDestructor? EntityDestructor,
    IReadOnlyDictionary<PropertyInfo, PropertyDestructuringConfiguration> PropertyDestructuringConfigurations);