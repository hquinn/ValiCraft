using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Providers.LitePrimitives.Generator.Helpers;

namespace ValiCraft.Providers.LitePrimitives.Generator;

public static class ValidatorInfoProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ValidatorInfoResult Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();
        
        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "VC001",
                    title: "Internal Error",
                    messageFormat: "Could not get syntax node for class",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                context.TargetNode.GetLocation());
            
            diagnostics.Add(diagnostic);

            return new ValidatorInfoResult(null, diagnostics.ToEquatableImmutableArray());
        }

        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "VC002",
                    title: "Internal Error",
                    messageFormat: "Could not get symbol for class",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                classDeclarationSyntax.GetLocation());
            
            diagnostics.Add(diagnostic);

            return new ValidatorInfoResult(null, diagnostics.ToEquatableImmutableArray());
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        
        // Must have the partial keyword
        if (!classDeclarationSyntax.IsPartial())
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "VC003",
                    title: "Missing partial keyword",
                    messageFormat: "Classes attributed with [GenerateValidator] must have the partial keyword",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                classDeclarationSyntax.GetLocation());
            
            diagnostics.Add(diagnostic);
        }
        
        // Must inherit from Validator<T>
        var requestTypeName = "";
        if (classSymbol.Inherits(FullyQualifiedNames.Classes.Validator, genericArgumentCount: 1))
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "VC004",
                    title: "Invalid Base Class",
                    messageFormat: $"Classes attributed with [GenerateValidator] must inherit from {FullyQualifiedNames.Classes.Validator}",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                classDeclarationSyntax.GetLocation());
            
            diagnostics.Add(diagnostic);
        }
        else
        {
            requestTypeName = $"global::{classSymbol.BaseType!.TypeArguments[0].ToDisplayString()}";
        }

        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();

        var className = classSymbol.Name;

        var validatorInfo = new ValidatorInfo(
            namespaceName,
            className,
            requestTypeName,
            new EquatableArray<RuleInfo>());

        return new ValidatorInfoResult(validatorInfo, diagnostics.ToEquatableImmutableArray());
    }
}

public record ValidatorInfoResult(
    ValidatorInfo? ValidatorInfo,
    EquatableArray<Diagnostic>? Diagnostics)
{
    public virtual bool Equals(ValidatorInfoResult? other)
    {
        if (other is null)
        {
            return false;
        }
        
        if ((ValidatorInfo is null && other.ValidatorInfo is not null) ||
            (ValidatorInfo is not null && other.ValidatorInfo is null))
        {
            return false;
        }

        if ((Diagnostics is null && other.Diagnostics is not null) ||
            (Diagnostics is not null && other.Diagnostics is null))
        {
            return false;
        }
        
        return ValidatorInfo!.Equals(other.ValidatorInfo) &&
               Diagnostics.Equals(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((ValidatorInfo != null ? ValidatorInfo.GetHashCode() : 0) * 397) ^ Diagnostics.GetHashCode();
        }
    }
}

public record ValidatorInfo(
    string NamespaceName,
    string ClassName,
    string RequestTypeName,
    EquatableArray<RuleInfo> Rules);

public record RuleInfo(
    string PropertyName);

public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new(Array.Empty<T>());

    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private readonly T[]? _array;

    /// <summary>
    /// Creates a new <see cref="EquatableArray{T}"/> instance.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray"/> to wrap.</param>
    public EquatableArray(T[] array)
    {
        _array = array;
    }

    /// <sinheritdoc/>
    public bool Equals(EquatableArray<T> array)
    {
        return AsSpan().SequenceEqual(array.AsSpan());
    }

    /// <sinheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> array && this.Equals(array);
    }

    /// <sinheritdoc/>
    public override int GetHashCode()
    {
        if (_array is not T[] array)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (T item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return _array.AsSpan();
    }

    /// <summary>
    /// Gets the underlying array if there is one
    /// </summary>
    public T[]? GetArray() => _array;

    /// <sinheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }    
    
    /// <sinheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    public int Count => _array?.Length ?? 0;

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}



public static class EquatableImmutableArrayExtensions
{
    public static EquatableArray<T> ToEquatableImmutableArray<T>(this IEnumerable<T> items) where T : IEquatable<T>
    {
        return new EquatableArray<T>(items.ToArray());
    }
}


