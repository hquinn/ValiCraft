using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

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

    public static string GetFullyQualifiedUnboundedName(this INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        var unboundGenericType = symbol.ConstructUnboundGenericType();

        return unboundGenericType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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

    public static INamedTypeSymbol? GetInterface(
        this INamedTypeSymbol classSymbol,
        string targetFullyQualifiedInterfaceName)
    {
        return classSymbol.AllInterfaces
            .FirstOrDefault(i =>
                i.OriginalDefinition
                    .ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) == targetFullyQualifiedInterfaceName);
    }

    public static EquatableArray<GenericParameterInfo> GetClassGenericParameters(
        this INamedTypeSymbol classSymbol,
        INamedTypeSymbol? implementedInterfaceSymbol)
    {
        var genericParameterInfos = new List<GenericParameterInfo>();

        if (!classSymbol.IsGenericType || implementedInterfaceSymbol is null)
        {
            return genericParameterInfos.ToEquatableImmutableArray();
        }

        foreach (var classTypeParameter in classSymbol.TypeParameters)
        {
            var inheritedPositions = new List<int>();

            for (var i = 0; i < implementedInterfaceSymbol.TypeArguments.Length; i++)
            {
                var interfaceTypeArgument = implementedInterfaceSymbol.TypeArguments[i];

                if (SymbolEqualityComparer.Default.Equals(classTypeParameter, interfaceTypeArgument))
                {
                    inheritedPositions.Add(i);
                }
            }

            var constraints = GenericConstraintsInfo.CreateFromTypeParameterSymbol(classTypeParameter);

            genericParameterInfos.Add(new GenericParameterInfo(
                new TypeInfo(classTypeParameter.Name, true, classTypeParameter.NullableAnnotation == NullableAnnotation.Annotated),
                inheritedPositions.ToEquatableImmutableArray(),
                constraints));
        }

        return genericParameterInfos.ToEquatableImmutableArray();
    }

    public static EquatableArray<GenericArgumentInfo> GetImplementedInterfaceGenericArguments(
        this INamedTypeSymbol? implementedInterfaceSymbol)
    {
        if (implementedInterfaceSymbol is null)
        {
            return EquatableArray<GenericArgumentInfo>.Empty;
        }

        var typeArguments = implementedInterfaceSymbol.TypeArguments
            .Select(x =>
            {
                var fullyQualifiedType = x.ToDisplayString();
                var isGeneric = x.TypeKind == TypeKind.TypeParameter;
                return new GenericArgumentInfo(fullyQualifiedType, isGeneric);
            })
            .ToEquatableImmutableArray();

        return typeArguments;
    }
}