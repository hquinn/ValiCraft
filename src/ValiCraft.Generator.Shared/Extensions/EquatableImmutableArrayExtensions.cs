using System;
using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Extensions;

public static class EquatableImmutableArrayExtensions
{
    public static EquatableArray<T> ToEquatableImmutableArray<T>(this IEnumerable<T> items) where T : IEquatable<T>
    {
        return new EquatableArray<T>(items.ToArray());
    }
}


