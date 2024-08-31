using Serilog.FluentDestructuring.Destructors;

namespace Serilog.FluentDestructuring;

internal abstract record PropertyDestructuringConfiguration(
    string PropertyAlias, 
    Func<object, bool>? ApplyDestructuringPredicate);

internal sealed record SimplePropertyDestructuringConfiguration(
    IPropertyDestructor PropertyDestructor, 
    string PropertyAlias,
    Func<object, bool>? ApplyDestructuringPredicate) : PropertyDestructuringConfiguration(PropertyAlias, ApplyDestructuringPredicate);

internal sealed record InnerEntityDestructuringConfiguration(
    EntityDestructuringConfiguration EntityConfiguration,
    string PropertyAlias,
    Func<object, bool>? ApplyDestructuringPredicate) : PropertyDestructuringConfiguration(PropertyAlias, ApplyDestructuringPredicate);