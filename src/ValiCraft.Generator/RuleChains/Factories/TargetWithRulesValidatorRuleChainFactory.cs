using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetWithRulesValidatorRuleChainFactory : IRuleChainFactory
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
        // The last invocation is the validator call — extract validator info
        var validatorInvocation = invocationChain.Last();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context);
        if (resolution is null)
        {
            return null;
        }

        // Process rules from index 1 to N-2 (skip Ensure and the validator call)
        var rules = RuleChainHelper.ProcessRuleInvocations(
            isAsyncValidator, invocationChain.Skip(1).Take(invocationChain.Count - 2), diagnostics, context);
        if (rules is null)
        {
            return null;
        }

        return new TargetWithRulesValidatorRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count + 1, // +1 for the validator call
            invocation.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray(),
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall);
    }
}
