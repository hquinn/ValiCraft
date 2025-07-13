using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public record ClassInfo(
    string Name,
    string Namespace,
    string Accessibility,
    string Modifiers,
    EquatableArray<GenericParameterInfo> GenericParameters,
    EquatableArray<GenericArgumentInfo> InterfaceGenericParameters,
    string FullyQualifiedWithoutGenerics,
    string FullyQualifiedUnboundedName)
{
    public static ClassInfo CreateFromSyntaxAndSymbols(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol? implementedInterfaceSymbol)
    {
        return new ClassInfo(
            classSymbol.Name,
            classSymbol.GetNamespace(),
            classSymbol.DeclaredAccessibility.ToCSharpKeyword(),
            classDeclarationSyntax.GetFullModifiers(),
            classSymbol.GetClassGenericParameters(implementedInterfaceSymbol),
            implementedInterfaceSymbol.GetImplementedInterfaceGenericArguments(),
            classSymbol.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric),
            classSymbol.GetFullyQualifiedUnboundedName());
    }
}