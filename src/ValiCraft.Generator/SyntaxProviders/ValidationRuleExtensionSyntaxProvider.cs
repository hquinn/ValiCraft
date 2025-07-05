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

public static class ValidationRuleExtensionSyntaxProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<ValidationRule> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();

        // If we can't get the class syntax and symbol, then return early
        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<ValidationRule>(diagnostics);
        }

        var succeeded = TryGetNameForExtensionMethod(
            classDeclarationSyntax!,
            classSymbol!,
            diagnostics,
            out var nameForExtensionMethod);

        succeeded &= TryGetValidationRuleInterface(
            classDeclarationSyntax!,
            classSymbol!,
            diagnostics,
            out var validationRuleInterface);

        succeeded &= TryGetIsValidMethod(
            classDeclarationSyntax!,
            classSymbol!,
            diagnostics,
            out var isValidMethodSignature);

        if (!succeeded)
        {
            return new ProviderResult<ValidationRule>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var classInfo = ClassInfo.CreateFromSyntaxAndSymbols(
            classDeclarationSyntax!, classSymbol!, validationRuleInterface);

        var defaultMessage = MessageInfo.CreateFromAttribute(
            classSymbol, KnownNames.Attributes.DefaultMessageAttribute);

        var rulePlaceholders = RulePlaceholderInfo.CreateFromRulePlaceholderAttributes(classSymbol!);

        var validationRuleInfo = new ValidationRule(
            classInfo,
            nameForExtensionMethod!,
            defaultMessage,
            isValidMethodSignature!,
            rulePlaceholders);

        return new ProviderResult<ValidationRule>(validationRuleInfo, diagnostics);
    }

    private static bool TryGetNameForExtensionMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out string? nameForExtensionMethod)
    {
        nameForExtensionMethod = classSymbol.GetAttributeStringArgument(
            KnownNames.Attributes.GenerateRuleExtensionAttribute);

        if (nameForExtensionMethod is null)
        {
            diagnostics.Add(
                DefinedDiagnostics.MissingValidationRuleExtensionName(classDeclarationSyntax.GetLocation()));
            return false;
        }

        return true;
    }

    private static bool TryGetValidationRuleInterface(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out INamedTypeSymbol? validationRuleInterface)
    {
        validationRuleInterface = classSymbol.GetInterface(KnownNames.Interfaces.IValidationRule);

        if (validationRuleInterface is null)
        {
            diagnostics.Add(DefinedDiagnostics.MissingIValidationRuleInterface(
                classDeclarationSyntax.GetLocation()));
            return false;
        }

        return true;
    }

    private static bool TryGetIsValidMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out MethodSignature? isValidMethodSignature)
    {
        var isValidMethod = classSymbol.GetMembers("IsValid")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic && m.DeclaredAccessibility == Accessibility.Public && m.Parameters.Any());

        if (isValidMethod is null)
        {
            diagnostics.Add(DefinedDiagnostics.MissingIsValidMethod(classDeclarationSyntax.GetLocation()));
            isValidMethodSignature = null;
            return false;
        }

        isValidMethodSignature = isValidMethod.GetMethodSignature();
        return true;
    }
}