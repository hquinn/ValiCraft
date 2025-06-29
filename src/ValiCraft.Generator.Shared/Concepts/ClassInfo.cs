using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Shared.Extensions;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Concepts;

public record ClassInfo
{
    public ClassInfo(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol? implementedInterfaceSymbol)
    {
        Name = classSymbol.Name;
        Namespace = classSymbol.GetNamespace();
        Accessibility = classSymbol.DeclaredAccessibility.ToCSharpKeyword();
        Modifiers = classDeclarationSyntax.GetFullModifiers();
        GenericParameters = classSymbol.GetClassGenericParameters(implementedInterfaceSymbol);
        FullyQualifiedWithoutGenerics = classSymbol.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric);
        FullyQualifiedUnboundedName = classSymbol.GetFullyQualifiedUnboundedName();
    }
    
    public string Name { get; init; }
    public string Namespace { get; init; }
    public string Accessibility { get; init; }
    public string Modifiers { get; init; }
    public EquatableArray<GenericParameterInfo> GenericParameters { get; init; }
    public string FullyQualifiedWithoutGenerics { get; init; }
    public string FullyQualifiedUnboundedName { get; init; }
}