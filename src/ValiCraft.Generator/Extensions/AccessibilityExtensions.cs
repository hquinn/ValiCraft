using System;
using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Extensions;

public static class AccessibilityExtensions
{
    public static string ToCSharpKeyword(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal", // Note the space
            Accessibility.ProtectedAndInternal => "private protected", // Note the space
            Accessibility.NotApplicable => string.Empty, // No C# keyword for this
            _ => throw new ArgumentOutOfRangeException(nameof(accessibility), $"Unknown accessibility: {accessibility}")
        };
    }
}