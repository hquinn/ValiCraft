using System;
using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Extensions;

public static class EquatableImmutableArrayExtensions
{
    public static EquatableArray<T> ToEquatableImmutableArray<T>(this IEnumerable<T> items) where T : IEquatable<T>
    {
        return new EquatableArray<T>(items.ToArray());
    }
}