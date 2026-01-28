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

    public static ProviderResult<Validator> TransformSync(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        return Transform(false, context, cancellationToken);
    }

    public static ProviderResult<Validator> TransformAsync(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        return Transform(true, context, cancellationToken);
    }

    public static ProviderResult<Validator> Transform(
        bool isAsyncValidator,
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();

        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        var succeeded = TryCheckPartialKeyword(classDeclarationSyntax!, diagnostics);
        succeeded &= TryGetRequestTypeName(isAsyncValidator, classDeclarationSyntax!, classSymbol!, diagnostics, out var requestTypeName);

        if (!succeeded)
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();
        
        var classInfo = ClassInfo.CreateFromSyntaxAndSymbols(classDeclarationSyntax!, classSymbol!);
        var ruleChains = RuleChainsSyntaxProvider.DiscoverRuleChains(
            isAsyncValidator,
            diagnostics,
            classDeclarationSyntax!,
            classSymbol!,
            context);

        var validator = new Validator(
            isAsyncValidator,
            classInfo,
            requestTypeName!,
            ruleChains,
            classDeclarationSyntax!.GetUsingDirectives());

        return new ProviderResult<Validator>(validator, diagnostics);
    }

    private static bool TryGetRequestTypeName(
        bool isAsyncValidator,
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out SymbolNameInfo? requestTypeName)
    {
        requestTypeName = null;
        if (!isAsyncValidator && !classSymbol.Inherits(KnownNames.Classes.Validator, 1))
        {
            diagnostics.Add(DefinedDiagnostics.MissingValidatorBaseClass(isAsyncValidator, classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }
        if (isAsyncValidator && !classSymbol.Inherits(KnownNames.Classes.AsyncValidator, 1))
        {
            diagnostics.Add(DefinedDiagnostics.MissingValidatorBaseClass(isAsyncValidator, classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }

        var typeArgument = classSymbol.BaseType!.TypeArguments[0];
        
        requestTypeName = new SymbolNameInfo(typeArgument);
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