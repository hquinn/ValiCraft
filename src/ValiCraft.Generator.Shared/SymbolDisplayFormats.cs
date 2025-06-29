using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Shared;

public static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat FormatWithoutGeneric = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None);

    public static SymbolDisplayFormat FormatAttributeWithoutParameters = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        parameterOptions: SymbolDisplayParameterOptions.None);
}