using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValiCraft.Generator.Types;

public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new([]);

    /// <summary>
    ///     The underlying <typeparamref name="T" /> array.
    /// </summary>
    private readonly T[]? _array;

    /// <summary>
    ///     Creates a new <see cref="EquatableArray{T}" /> instance.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray" /> to wrap.</param>
    public EquatableArray(T[] array)
    {
        _array = array;
    }

    public EquatableArray(IEnumerable<T> array)
    {
        _array = array.ToArray();
    }

    public T this[int index] => _array![index];

    /// <sinheritdoc />
    public bool Equals(EquatableArray<T> array)
    {
        return AsSpan().SequenceEqual(array.AsSpan());
    }

    /// <sinheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> array && Equals(array);
    }

    /// <sinheritdoc />
    public override int GetHashCode()
    {
        if (_array is not { } array)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (var item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    ///     Returns a <see cref="ReadOnlySpan{T}" /> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}" /> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return _array.AsSpan();
    }

    /// <summary>
    ///     Gets the underlying array if there is one
    /// </summary>
    public T[]? GetArray()
    {
        return _array;
    }

    /// <sinheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    /// <sinheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    public int Count => _array?.Length ?? 0;

    /// <summary>
    ///     Checks whether two <see cref="EquatableArray{T}" /> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}" /> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}" /> value.</param>
    /// <returns>Whether <paramref name="left" /> and <paramref name="right" /> are equal.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Checks whether two <see cref="EquatableArray{T}" /> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}" /> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}" /> value.</param>
    /// <returns>Whether <paramref name="left" /> and <paramref name="right" /> are not equal.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}