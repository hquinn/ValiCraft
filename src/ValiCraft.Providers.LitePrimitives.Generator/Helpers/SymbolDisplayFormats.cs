using Microsoft.CodeAnalysis;

namespace ValiCraft.Providers.LitePrimitives.Generator.Helpers;

public static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat FormatWithoutGeneric = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None);
}