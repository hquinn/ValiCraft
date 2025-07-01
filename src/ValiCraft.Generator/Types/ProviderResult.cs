using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.Types;

public record ProviderResult<TValue> where TValue : class
{
    public ProviderResult(EquatableArray<Diagnostic> diagnostics)
        : this(null, diagnostics)
    {
    }

    public ProviderResult(List<Diagnostic> diagnostics)
        : this(null, diagnostics.ToEquatableImmutableArray())
    {
    }

    public ProviderResult(TValue? value, List<Diagnostic> diagnostics)
        : this(value, diagnostics.ToEquatableImmutableArray())
    {
    }

    public ProviderResult(TValue? value, EquatableArray<Diagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics;
    }
    
    public TValue? Value { get; init; }
    public EquatableArray<Diagnostic> Diagnostics { get; init; }
}