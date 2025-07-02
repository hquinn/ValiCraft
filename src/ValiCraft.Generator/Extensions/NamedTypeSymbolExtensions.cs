using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;
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
        AttributeData? attributeData = symbol.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == attributeFullName);

        if (attributeData == null)
        {
            return null;
        }

        if (attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            return null;
        }

        TypedConstant firstArgument = attributeData.ConstructorArguments[0];

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
            
            var constraints = GetConstraintsAsString(classTypeParameter);
            
            genericParameterInfos.Add(new GenericParameterInfo(
                classTypeParameter.Name, 
                inheritedPositions.ToEquatableImmutableArray(), 
                constraints));
        }
        
        return genericParameterInfos.ToEquatableImmutableArray();
    }
    
    public static string GetConstraintsAsString(ITypeParameterSymbol typeParameter)
    {
        var constraints = new List<string>();

        // Check for class, struct, notnull, unmanaged, and new() constraints
        if (typeParameter.HasReferenceTypeConstraint) constraints.Add("class");
        if (typeParameter.HasValueTypeConstraint) constraints.Add("struct");
        if (typeParameter.HasNotNullConstraint) constraints.Add("notnull");
        if (typeParameter.HasUnmanagedTypeConstraint) constraints.Add("unmanaged");
        
        // Add any type constraints (e.g., where T : IMyInterface)
        foreach (var constraintType in typeParameter.ConstraintTypes)
        {
            constraints.Add(constraintType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (typeParameter.HasConstructorConstraint) constraints.Add("new()");

        // If there are no constraints, return an empty string.
        if (constraints.Count == 0) return string.Empty;

        // Otherwise, build the full "where" clause.
        return $"where {typeParameter.Name} : {string.Join(", ", constraints)}";
    }
}