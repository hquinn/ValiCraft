using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.Concepts;

public record ClassInfo(
    string Name,
    string Namespace,
    string Modifiers)
{
    public static ClassInfo CreateFromSyntaxAndSymbols(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol)
    {
        return new ClassInfo(
            classSymbol.Name,
            classSymbol.GetNamespace(),
            classDeclarationSyntax.GetFullModifiers());
    }
}