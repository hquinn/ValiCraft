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
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax!, classSymbol!, diagnostics, out var isAsyncValidator, out var requestTypeName);

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
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out bool isAsyncValidator,
        out SymbolNameInfo? requestTypeName)
    {
        requestTypeName = null;
        isAsyncValidator = false;
        
        // Detect sync vs async from base class
        var inheritsValidator = classSymbol.Inherits(KnownNames.Classes.Validator, 1);
        var inheritsAsyncValidator = classSymbol.Inherits(KnownNames.Classes.AsyncValidator, 1);
        
        if (!inheritsValidator && !inheritsAsyncValidator)
        {
            diagnostics.Add(DefinedDiagnostics.MissingValidatorBaseClass(false, classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }
        
        isAsyncValidator = inheritsAsyncValidator;

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