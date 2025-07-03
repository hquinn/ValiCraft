using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Extensions;

public static class SyntaxContextExtensions
{
    public static bool TryGetClassNodeAndSymbol(
        this GeneratorAttributeSyntaxContext context,
        List<Diagnostic> diagnostics,
        out ClassDeclarationSyntax? classDeclarationSyntax,
        out INamedTypeSymbol? classSymbol)
    {
        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetNode is not ClassDeclarationSyntax syntax)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "VC001",
                    "Internal Error",
                    "Could not get syntax node for class",
                    "ValiCraft",
                    DiagnosticSeverity.Error,
                    true),
                context.TargetNode.GetLocation());

            diagnostics.Add(diagnostic);

            classDeclarationSyntax = null;
            classSymbol = null;
            return false;
        }

        // This shouldn't really happen, but check for this condition
        // If this happens, might as well return early
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "VC002",
                    "Internal Error",
                    "Could not get symbol for class",
                    "ValiCraft",
                    DiagnosticSeverity.Error,
                    true),
                syntax.GetLocation());

            diagnostics.Add(diagnostic);

            classDeclarationSyntax = null;
            classSymbol = null;
            return false;
        }

        classDeclarationSyntax = syntax;
        classSymbol = symbol;
        return true;
    }
}