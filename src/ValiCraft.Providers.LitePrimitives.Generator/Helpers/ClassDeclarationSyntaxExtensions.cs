using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Providers.LitePrimitives.Generator.Helpers;

public static class ClassDeclarationSyntaxExtensions
{
    public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    }
}