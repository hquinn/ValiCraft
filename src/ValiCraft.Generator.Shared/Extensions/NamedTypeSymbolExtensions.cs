using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Extensions;

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

    public static string? GetAttributeStringArgument(this INamedTypeSymbol symbol, string attributeFullName)
    {
        AttributeData? attributeData = symbol.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == attributeFullName);

        if (attributeData == null)
        {
            return null;
        }

        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return null;
        }

        TypedConstant firstArgument = attributeData.ConstructorArguments[0];

        if (firstArgument.Kind == TypedConstantKind.Primitive && firstArgument.Value is string stringValue)
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
            
            genericParameterInfos.Add(new GenericParameterInfo(classTypeParameter.Name, inheritedPositions));
        }
        
        return genericParameterInfos.ToEquatableImmutableArray();
    }
}