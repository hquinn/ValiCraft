using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Extensions;

public static class NamedTypeSymbolExtensions
{
    public static bool Inherits(
        this INamedTypeSymbol symbol,
        string fullyQualifiedBaseTypeName,
        int genericArgumentCount)
    {
        if (symbol.BaseType is null)
        {
            return false;
        }

        if (symbol.BaseType.OriginalDefinition.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) !=
            fullyQualifiedBaseTypeName)
        {
            return false;
        }

        return symbol.BaseType.TypeArguments.Length == genericArgumentCount;
    }

    public static string? GetAttributeStringArgument(
        this INamedTypeSymbol symbol,
        string attributeFullName)
    {
        var attributeData = symbol.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == attributeFullName);

        if (attributeData == null)
        {
            return null;
        }

        if (attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            return null;
        }

        var firstArgument = attributeData.ConstructorArguments[0];

        if (firstArgument is { Kind: TypedConstantKind.Primitive, Value: string stringValue })
        {
            return stringValue;
        }

        return null;
    }

    public static string GetNamespace(this INamedTypeSymbol symbol)
    {
        return symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();
    }
}