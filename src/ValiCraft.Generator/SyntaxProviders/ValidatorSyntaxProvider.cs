using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SyntaxProviders;

public static class ValidatorSyntaxProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<Validator> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();

        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        var succeeded = TryCheckPartialKeyword(classDeclarationSyntax!, diagnostics);
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax!, classSymbol!, diagnostics, out var requestTypeName);

        if (!succeeded)
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();
        
        var classInfo = ClassInfo.CreateFromSyntaxAndSymbols(classDeclarationSyntax!, classSymbol!, null);
        var ruleChains = RuleChainsSyntaxProvider.DiscoverRuleChains(
            diagnostics,
            classDeclarationSyntax!,
            classSymbol!,
            context);

        var validator = new Validator(
            classInfo,
            requestTypeName!,
            ruleChains,
            classDeclarationSyntax!.GetUsingDirectives());

        return new ProviderResult<Validator>(validator, diagnostics);
    }

    private static bool TryGetRequestTypeName(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out string? requestTypeName)
    {
        requestTypeName = null;
        if (!classSymbol.Inherits(KnownNames.Classes.Validator, 1))
        {
            diagnostics.Add(DefinedDiagnostics.MissingValidatorBaseClass(classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }

        requestTypeName = $"global::{classSymbol.BaseType!.TypeArguments[0].ToDisplayString()}";
        return true;
    }

    private static bool TryCheckPartialKeyword(
        ClassDeclarationSyntax classDeclarationSyntax,
        List<DiagnosticInfo> diagnostics)
    {
        if (!classDeclarationSyntax.IsPartial())
        {
            diagnostics.Add(DefinedDiagnostics.MissingPartialKeyword(classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }

        return true;
    }
}