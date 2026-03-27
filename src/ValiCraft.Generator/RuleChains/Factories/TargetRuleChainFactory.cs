using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetRuleChainFactory : IRuleChainFactory
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
        // Skip the Ensure method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(isAsyncValidator, invocationChain.Skip(1), diagnostics, context);
        if (rules is null)
        {
            return null;
        }

        // Now that we have all the rules in the chain, we can now create the rule chain
        return new TargetRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count,
            invocation?.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray());
    }
}
