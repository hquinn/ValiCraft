using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.Types;

public record ProviderResult<TValue>(TValue? Value, EquatableArray<DiagnosticInfo> Diagnostics)
    where TValue : class
{
    public ProviderResult(List<DiagnosticInfo> diagnostics)
        : this(null, diagnostics.ToEquatableImmutableArray())
    {
    }

    public ProviderResult(TValue? value, List<DiagnosticInfo> diagnostics)
        : this(value, diagnostics.ToEquatableImmutableArray())
    {
    }
}