using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ValiCraft.Generator.Shared;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Extensions;
using ValiCraft.Generator.Shared.Types;
using ValiCraft.Providers.LitePrimitives.Generator.Concepts;
using ValiCraft.Rules.Generator.Shared.Concepts;

// For MethodSignatureInfo

namespace ValiCraft.Providers.LitePrimitives.Generator;

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
        
        bool succeeded = TryCheckPartialKeyword(classDeclarationSyntax, diagnostics);
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax, classSymbol, diagnostics, out var requestTypeName);

        if (!succeeded)
        {
            return new ProviderResult<ValidatorInfo>(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var invocations = DiscoverRuleInvocations(context, classDeclarationSyntax);
        var classInfo = new ClassInfo(classDeclarationSyntax, classSymbol, null);
        
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

    foreach (var invocation in defineRulesMethod.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax ruleMemberAccess ||
            ruleMemberAccess.Expression is not InvocationExpressionSyntax ensureInvocation ||
            ensureInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is not SimpleLambdaExpressionSyntax lambda ||
            lambda.Body is not MemberAccessExpressionSyntax propertyAccess)
        {
            continue; // Not the builder.Ensure(...).RuleMethod(...) pattern
        }
        
        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

        NameAndTypeInfo property;
        
        if (context.SemanticModel.GetSymbolInfo(propertyAccess).Symbol is IPropertySymbol propertySymbol)
        {
            property = new NameAndTypeInfo(
                propertyAccess.Name.Identifier.ValueText,
                propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }
        else
        {
            property = new NameAndTypeInfo(propertyAccess.Name.Identifier.ValueText, "ERROR");
        }

        // The types of the arguments can often be resolved even if the method call itself cannot.
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

            if (mapToValidationRuleAttribute is not null)
            {
                var constructorArguments = mapToValidationRuleAttribute.ConstructorArguments;
                
                if (constructorArguments.Length >= 2)
                {
                    fullyQualifiedValidationRule = (constructorArguments[0].Value as INamedTypeSymbol)
                        ?.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) ?? string.Empty;
                    validationRuleGenericFormat = constructorArguments[1].Value as string ?? string.Empty;
                }
            }
        }
        
        ruleInvocations.Add(new RuleInvocation(
            property,
            ruleMemberAccess.Name.Identifier.ValueText, 
            ruleInvocationArguments,
            fullyQualifiedValidationRule,
            validationRuleGenericFormat));
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