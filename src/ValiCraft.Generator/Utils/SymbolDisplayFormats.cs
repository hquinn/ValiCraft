using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Utils;

public static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat FormatWithoutGeneric = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static SymbolDisplayFormat FormatAttributeWithoutParameters = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
}