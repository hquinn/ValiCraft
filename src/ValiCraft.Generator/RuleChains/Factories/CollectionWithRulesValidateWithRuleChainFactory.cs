using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Rules.Builders;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionWithRulesValidateWithRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(
        bool isAsyncValidator,
        ValidationTarget @object,
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        // Resolve the element type from EnsureEach
        var elementTypeInfo = GetElementTypeInfo(invocation, context);
        if (elementTypeInfo is null)
        {
            return null;
        }

        // The last invocation is ValidateWith
        var validateWithInvocation = invocationChain.Last();
        var argumentExpression = validateWithInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (argumentExpression is null)
        {
            return null;
        }

        var validatorExpression = argumentExpression.ToString();

        // Determine if the validator argument is an IAsyncValidator
        var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
        var isAsyncValidatorCall = typeInfo.Type.IsAsyncValidatorType();

        // Process rules from index 1 to N-2
        RuleBuilder? ruleBuilder = null;
        var rules = new List<Rule>();

        foreach (var ruleInvocation in invocationChain.Skip(1).Take(invocationChain.Count - 2))
        {
            var result = TargetRuleChainFactory.ProcessNextInChain(
                isAsyncValidator, ruleBuilder, ruleInvocation, rules, diagnostics, context);

            if (result is null)
            {
                return null;
            }

            ruleBuilder = result;
        }

        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }

        return new CollectionWithRulesValidateWithRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count + 1,
            invocation.GetOnFailureModeFromSyntax(),
            elementTypeInfo,
            rules.ToEquatableImmutableArray(),
            validatorExpression,
            isAsyncValidatorCall);
    }

    private static TypeInfo? GetElementTypeInfo(
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
        {
            return null;
        }

        if (methodSymbol.TypeArguments.Length == 0)
        {
            return null;
        }

        var elementType = methodSymbol.TypeArguments[0];

        return new TypeInfo(
            elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            elementType.NullableAnnotation == NullableAnnotation.Annotated);
    }
}
