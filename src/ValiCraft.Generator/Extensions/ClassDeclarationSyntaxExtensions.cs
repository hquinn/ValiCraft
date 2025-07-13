using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Extensions;

public static class ClassDeclarationSyntaxExtensions
{
    public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    }

    public static string GetFullModifiers(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (classDeclarationSyntax.Modifiers.Count != 0)
        {
            return classDeclarationSyntax.Modifiers.ToFullString().Trim();
        }

        return string.Empty;
    }

    public static EquatableArray<string> GetUsingDirectives(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax!.SyntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Select(u => u.ToString())
            .ToEquatableImmutableArray();
    }
}