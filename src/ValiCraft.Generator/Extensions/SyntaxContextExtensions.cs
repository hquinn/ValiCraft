using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Extensions;

public static class SyntaxContextExtensions
{
    public static bool TryGetClassNodeAndSymbol(
        this GeneratorAttributeSyntaxContext context,
        List<DiagnosticInfo> diagnostics,
        out ClassDeclarationSyntax? classDeclarationSyntax,
        out INamedTypeSymbol? classSymbol)
    {
        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetNode is not ClassDeclarationSyntax syntax)
        {
            diagnostics.Add(DefinedDiagnostics.CouldNotFindDeclaredSyntax(context.TargetNode.GetLocation()));
            classDeclarationSyntax = null;
            classSymbol = null;
            return false;
        }

        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
        {
            diagnostics.Add(DefinedDiagnostics.CouldNotFindSymbol(syntax.GetLocation()));
            classDeclarationSyntax = null;
            classSymbol = null;
            return false;
        }

        classDeclarationSyntax = syntax;
        classSymbol = symbol;
        return true;
    }
}