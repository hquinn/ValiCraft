using Microsoft.CodeAnalysis;

namespace ValiCraft.Rules.Generator.Helpers;

public static class ClassSymbolExtensions
{
    public static bool Inherits(
        this INamedTypeSymbol classSymbol,
        string fullyQualifiedBaseTypeName,
        int genericArgumentCount)
    {
        if (classSymbol.BaseType is null)
        {
            return false;
        }

        if (classSymbol.BaseType.OriginalDefinition.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) !=
            fullyQualifiedBaseTypeName)
        {
            return false;
        }
        
        return classSymbol.BaseType.TypeArguments.Length == genericArgumentCount;
    }
}