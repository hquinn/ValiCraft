using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SyntaxProviders;

public static class AsyncValidationRuleExtensionSyntaxProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<AsyncValidationRule> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();

        // If we can't get the class syntax and symbol, then return early
        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<AsyncValidationRule>(diagnostics);
        }

        var succeeded = TryGetNameForExtensionMethod(
            classDeclarationSyntax!,
            classSymbol!,
            diagnostics,
            out var nameForExtensionMethod);

        succeeded &= TryGetAsyncValidationRuleInterface(
            classDeclarationSyntax!,
            classSymbol!,
            diagnostics,
            out var asyncValidationRuleInterface);

        succeeded &= TryGetIsValidAsyncMethod(classSymbol!,
            out var isValidAsyncMethodSignature);

        succeeded &= RulePlaceholder.TryCreateFromRulePlaceholderAttributes(
            classSymbol!,
            isValidAsyncMethodSignature!,
            diagnostics,
            out var rulePlaceholders);

        if (!succeeded)
        {
            return new ProviderResult<AsyncValidationRule>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var classInfo = ClassInfo.CreateFromSyntaxAndSymbols(
            classDeclarationSyntax!, classSymbol!, asyncValidationRuleInterface);

        var defaultMessage = MessageInfo.CreateFromAttribute(
            classSymbol, KnownNames.Attributes.DefaultMessageAttribute);

        var defaultErrorCode = MessageInfo.CreateFromAttribute(
            classSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute);

        var asyncValidationRule = new AsyncValidationRule(
            classInfo,
            nameForExtensionMethod!,
            defaultMessage,
            defaultErrorCode,
            isValidAsyncMethodSignature!,
            rulePlaceholders);

        return new ProviderResult<AsyncValidationRule>(asyncValidationRule, diagnostics);
    }

    private static bool TryGetNameForExtensionMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out string? nameForExtensionMethod)
    {
        nameForExtensionMethod = classSymbol.GetAttributeStringArgument(
            KnownNames.Attributes.GenerateAsyncRuleExtensionAttribute);

        if (nameForExtensionMethod is null)
        {
            diagnostics.Add(
                DefinedDiagnostics.MissingValidationRuleExtensionName(classDeclarationSyntax.GetLocation()));
            return false;
        }

        return true;
    }

    private static bool TryGetAsyncValidationRuleInterface(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out INamedTypeSymbol? asyncValidationRuleInterface)
    {
        asyncValidationRuleInterface = classSymbol.GetInterface(KnownNames.Interfaces.IAsyncValidationRule);

        if (asyncValidationRuleInterface is null)
        {
            diagnostics.Add(DefinedDiagnostics.MissingIAsyncValidationRuleInterface(
                classDeclarationSyntax.GetLocation()));
            return false;
        }

        return true;
    }

    private static bool TryGetIsValidAsyncMethod(
        INamedTypeSymbol classSymbol,
        out MethodSignature? isValidAsyncMethodSignature)
    {
        var isValidAsyncMethod = classSymbol.GetMembers(KnownNames.Methods.IsValidAsync)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic && m.DeclaredAccessibility == Accessibility.Public && m.Parameters.Any());

        if (isValidAsyncMethod is null)
        {
            // No need for diagnostics, as this is not valid C# code anyway
            isValidAsyncMethodSignature = null;
            return false;
        }

        isValidAsyncMethodSignature = isValidAsyncMethod.GetMethodSignature();
        return true;
    }
}
