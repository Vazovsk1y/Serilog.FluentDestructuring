using System.Collections;

namespace Serilog.FluentDestructuring.UnitTests.Models;

internal class FluentDestructuringPolicyModel
{
    public int? NullProperty { get; init; }
    
    public int? NullProperty2 { get; init; }
    
    public string? StringProperty { get; init; }

    public IEnumerable<InnerEntityPropertyModel> IEnumerable { get; init; } = [];

    public Dictionary<Guid, InnerEntityPropertyModel> Dictionary { get; init; } = [];

    public IEnumerable IEnumerableAsScalar { get; init; } = Array.Empty<object>();
}