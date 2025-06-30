using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ValiCraft.Generator.Shared;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Extensions;
using ValiCraft.Generator.Shared.Types;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator;

public static class ValidatorInfoProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<ValidatorInfo> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();

        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<ValidatorInfo>(diagnostics);
        }
        
        bool succeeded = TryCheckPartialKeyword(classDeclarationSyntax!, diagnostics);
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax!, classSymbol!, diagnostics, out var requestTypeName);

        if (!succeeded)
        {
            return new ProviderResult<ValidatorInfo>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var invocations = DiscoverRuleInvocations(context, classDeclarationSyntax!);
        var classInfo = new ClassInfo(classDeclarationSyntax!, classSymbol!, null);
        
        var validatorInfo = new ValidatorInfo(
            classInfo,
            requestTypeName!,
            invocations.ToEquatableImmutableArray());

        return new ProviderResult<ValidatorInfo>(validatorInfo, diagnostics);
    }

    private static List<RuleInvocation> DiscoverRuleInvocations(
        GeneratorAttributeSyntaxContext context,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var ruleInvocations = new List<RuleInvocation>();

        var defineRulesMethod = classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == "DefineRules");

        if (defineRulesMethod?.Body is null)
        {
            return ruleInvocations;
        }

        // 1. Instead of looping over all invocations, we loop over each statement in the method.
        // This allows us to treat each fluent chain as a single unit.
        foreach (var statement in defineRulesMethod.Body.Statements.OfType<ExpressionStatementSyntax>())
        {
            // A valid chain must end with a method call.
            if (statement.Expression is not InvocationExpressionSyntax outermostInvocation)
            {
                continue;
            }

            // 2. "Walk the chain" backwards from the outermost call (e.g., .IsGenericRule2)
            // to the innermost call (e.g., .Ensure) to collect all parts of the chain.
            var invocationChain = new List<InvocationExpressionSyntax>();
            var currentExpression = (ExpressionSyntax)outermostInvocation;

            while (currentExpression is InvocationExpressionSyntax currentInvocation)
            {
                invocationChain.Add(currentInvocation);
                if (currentInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    currentExpression = memberAccess.Expression;
                }
                else
                {
                    break; // We've reached the start of the chain (the 'builder' identifier).
                }
            }

            // The collected chain is backwards, so we reverse it to get the correct execution order.
            invocationChain.Reverse();

            // 3. A valid chain must start with an 'Ensure(...)' call.
            if (invocationChain.Count < 2) continue; // Must have at least Ensure() and one rule.

            var ensureInvocation = invocationChain[0];
            var ensureMemberAccess = ensureInvocation.Expression as MemberAccessExpressionSyntax;
            if (ensureMemberAccess?.Name.Identifier.ValueText != "Ensure")
            {
                continue;
            }

            // 4. Extract the property information from the 'Ensure' call's lambda.
            // This property applies to ALL subsequent rule calls in this chain.
            if (ensureInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is not SimpleLambdaExpressionSyntax lambda ||
                lambda.Body is not MemberAccessExpressionSyntax propertyAccess)
            {
                continue; // Ensure() call is malformed.
            }

            NameAndTypeInfo property;
            if (context.SemanticModel.GetSymbolInfo(propertyAccess).Symbol is IPropertySymbol propertySymbol)
            {
                property = new NameAndTypeInfo(
                    propertyAccess.Name.Identifier.ValueText,
                    propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
            else
            {
                // If we can't resolve the property, we can't safely process this chain.
                // You could add a diagnostic here.
                continue;
            }

            // 5. Now, process each *actual* rule call in the chain, skipping the initial Ensure() call.
            foreach (var invocation in invocationChain.Skip(1))
            {
                // This is your existing logic for analyzing a single invocation. It works perfectly here.
                var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

                var ruleInvocationArguments = invocation.ArgumentList.Arguments
                    .Select(arg =>
                    {
                        var name = arg.Expression.ToString();
                        var type = context.SemanticModel.GetTypeInfo(arg.Expression).Type;

                        return type is not null
                            ? new NameAndTypeInfo(name, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                            : new NameAndTypeInfo(name, "ERROR");
                    }).ToEquatableImmutableArray();

                var validationRuleGenericFormat = string.Empty;
                var fullyQualifiedValidationRule = string.Empty;

                if (methodSymbol is not null && methodSymbol.IsExtensionMethod)
                {
                    var attributeDisplayFormat = SymbolDisplayFormats.FormatAttributeWithoutParameters;
                    var mapToValidationRuleAttribute = methodSymbol
                        .GetAttributes()
                        .FirstOrDefault(attributeData
                            => attributeData.AttributeClass?.ToDisplayString(attributeDisplayFormat) ==
                               FullyQualifiedNames.Attributes.MapToValidationRuleAttribute);

                    if (mapToValidationRuleAttribute is not null && mapToValidationRuleAttribute.ConstructorArguments.Length >= 2)
                    {
                        fullyQualifiedValidationRule = (mapToValidationRuleAttribute.ConstructorArguments[0].Value as INamedTypeSymbol)
                            ?.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) ?? string.Empty;
                        validationRuleGenericFormat = mapToValidationRuleAttribute.ConstructorArguments[1].Value as string ?? string.Empty;
                    }
                }

                var currentRuleMemberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                ruleInvocations.Add(new RuleInvocation(
                    property, // Use the common property for all rules in this chain.
                    currentRuleMemberAccess.Name.Identifier.ValueText,
                    ruleInvocationArguments,
                    fullyQualifiedValidationRule,
                    validationRuleGenericFormat));
            }
        }

        return ruleInvocations;
    }

    private static bool TryGetRequestTypeName(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out string? requestTypeName)
    {
        requestTypeName = null;
        if (!classSymbol.Inherits(FullyQualifiedNames.Classes.Validator, genericArgumentCount: 1))
        {
            diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor("VC004", "Invalid Base Class", $"Classes attributed with [GenerateValidator] must inherit from Validator<TRequest>", "ValiCraft", DiagnosticSeverity.Error, true),
                classDeclarationSyntax.GetLocation()));
            return false;
        }

        requestTypeName = $"global::{classSymbol.BaseType!.TypeArguments[0].ToDisplayString()}";
        return true;
    }

    private static bool TryCheckPartialKeyword(ClassDeclarationSyntax classDeclarationSyntax, List<Diagnostic> diagnostics)
    {
        if (!classDeclarationSyntax.IsPartial())
        {
            diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor("VC003", "Missing partial keyword", "Classes attributed with [GenerateValidator] must have the partial keyword", "ValiCraft", DiagnosticSeverity.Error, true),
                classDeclarationSyntax.GetLocation()));
            return false;
        }
        return true;
    }
}