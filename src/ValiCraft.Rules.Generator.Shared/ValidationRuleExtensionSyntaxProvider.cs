using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ValiCraft.Generator.Shared;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Extensions;
using ValiCraft.Generator.Shared.Types;
using ValiCraft.Rules.Generator.Shared.Concepts;

namespace ValiCraft.Rules.Generator.Shared;

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
        {
            return new ProviderResult<ValidationRuleInfo>(diagnostics);
        }

        var succeeded = TryGetNameForExtensionMethod(classDeclarationSyntax!, classSymbol!, diagnostics, out var nameForExtensionMethod);
        succeeded &= TryGetValidationRuleInterface(classDeclarationSyntax!, classSymbol!, diagnostics, out var validationRuleInterface);
        succeeded &= TryGetIsValidMethod(classDeclarationSyntax!, classSymbol!, diagnostics, out var isValidMethodSignature);

        if (!succeeded)
        {
            return new ProviderResult<ValidationRuleInfo>(diagnostics);
        }
        
        cancellationToken.ThrowIfCancellationRequested();

        var classInfo = new ClassInfo(classDeclarationSyntax!, classSymbol!, validationRuleInterface);
        
        var validationRuleInfo = new ValidationRuleInfo(
            classInfo,
            nameForExtensionMethod!,
            isValidMethodSignature!);

        return new ProviderResult<ValidationRuleInfo>(validationRuleInfo, diagnostics);
    }

    private static bool TryGetNameForExtensionMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out string? nameForExtensionMethod)
    {        
        nameForExtensionMethod = classSymbol.GetAttributeStringArgument(FullyQualifiedNames.Attributes.GenerateRuleExtensionAttribute);

        if (nameForExtensionMethod is null)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "VC005",
                    title: "Validation Rule Extension Name not specified",
                    messageFormat: "Could not get name for Validation Rule Extension",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
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
                    id: "VC006",
                    title: "Missing IValidationRule interface",
                    messageFormat: "Could not find IValidationRule interface on class marked with [GenerateRuleExtension]",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
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
                    id: "VC007",
                    title: "Missing IsValid method",
                    messageFormat: "Could not find IsValid method",
                    category: "ValiCraft",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                classDeclarationSyntax.GetLocation());

            diagnostics.Add(diagnostic);
            isValidMethodSignature = null;
            return false;
        }

        isValidMethodSignature = isValidMethod.GetMethodSignature();
        return true;
    }
}