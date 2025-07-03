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

    public static ProviderResult<ValidationRuleInfo> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();

        // If we can't get the class syntax and symbol, then return early
        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
            return new ProviderResult<ValidationRuleInfo>(diagnostics);

        var succeeded = TryGetNameForExtensionMethod(classDeclarationSyntax!, classSymbol!, diagnostics,
            out var nameForExtensionMethod);
        succeeded &= TryGetValidationRuleInterface(classDeclarationSyntax!, classSymbol!, diagnostics,
            out var validationRuleInterface);
        succeeded &= TryGetIsValidMethod(classDeclarationSyntax!, classSymbol!, diagnostics,
            out var isValidMethodSignature);

        if (!succeeded) return new ProviderResult<ValidationRuleInfo>(diagnostics);

        cancellationToken.ThrowIfCancellationRequested();

        var classInfo = new ClassInfo(classDeclarationSyntax!, classSymbol!, validationRuleInterface);
        var defaultMessage =
            MessageInfo.CreateFromAttribute(classSymbol, FullyQualifiedNames.Attributes.DefaultMessageAttribute);
        var rulePlaceholders = RulePlaceholderInfo.CreateFromRulePlaceholderAttributes(classSymbol!);

        var validationRuleInfo = new ValidationRuleInfo(
            classInfo,
            nameForExtensionMethod!,
            defaultMessage,
            isValidMethodSignature!,
            rulePlaceholders);

        return new ProviderResult<ValidationRuleInfo>(validationRuleInfo, diagnostics);
    }

    private static bool TryGetNameForExtensionMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out string? nameForExtensionMethod)
    {
        nameForExtensionMethod =
            classSymbol.GetAttributeStringArgument(FullyQualifiedNames.Attributes.GenerateRuleExtensionAttribute);

        if (nameForExtensionMethod is null)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "VC005",
                    "Validation Rule Extension Name not specified",
                    "Could not get name for Validation Rule Extension",
                    "ValiCraft",
                    DiagnosticSeverity.Error,
                    true),
                classDeclarationSyntax.GetLocation());

            diagnostics.Add(diagnostic);

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
        validationRuleInterface = classSymbol.GetInterface(FullyQualifiedNames.Interfaces.IValidationRule);

        if (validationRuleInterface is null)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "VC006",
                    "Missing IValidationRule interface",
                    "Could not find IValidationRule interface on class marked with [GenerateRuleExtension]",
                    "ValiCraft",
                    DiagnosticSeverity.Error,
                    true),
                classDeclarationSyntax.GetLocation());

            diagnostics.Add(diagnostic);
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
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "VC007",
                    "Missing IsValid method",
                    "Could not find IsValid method",
                    "ValiCraft",
                    DiagnosticSeverity.Error,
                    true),
                classDeclarationSyntax.GetLocation());

            diagnostics.Add(diagnostic);
            isValidMethodSignature = null;
            return false;
        }

        isValidMethodSignature = isValidMethod.GetMethodSignature();
        return true;
    }
}