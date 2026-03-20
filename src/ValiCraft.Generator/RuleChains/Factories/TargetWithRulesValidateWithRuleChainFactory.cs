using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Rules.Builders;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetWithRulesValidateWithRuleChainFactory : IRuleChainFactory
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
        // The last invocation is ValidateWith — process everything between Ensure and ValidateWith as rules
        var validateWithInvocation = invocationChain.Last();
        var argumentExpression = validateWithInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (argumentExpression is null)
        {
            return null;
        }

        var validatorExpression = argumentExpression.ToString();

        // Determine if the validator argument is an IAsyncValidator
        var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
        var isAsyncValidatorCall = CollectionValidateWithRuleChainFactory.IsAsyncValidatorType(typeInfo.Type);

        // Process rules from index 1 to N-2 (skip Ensure and ValidateWith)
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

        // Add the last rule into the rule list
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }

        return new TargetWithRulesValidateWithRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count + 1, // +1 for the ValidateWith
            invocation.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray(),
            validatorExpression,
            isAsyncValidatorCall);
    }
}
