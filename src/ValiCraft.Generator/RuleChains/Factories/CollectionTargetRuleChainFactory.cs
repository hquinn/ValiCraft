using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionTargetRuleChainFactory : IRuleChainFactory
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
        // Resolve the element type from the EnsureEach method's TTarget generic argument.
        var elementTypeInfo = invocation.GetElementTypeInfo(context);
        if (elementTypeInfo is null)
        {
            return null;
        }

        // Skip the EnsureEach method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            isAsyncValidator, invocationChain.Skip(1), diagnostics, context);
        if (rules is null)
        {
            return null;
        }

        return new CollectionTargetRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count,
            invocation.GetOnFailureModeFromSyntax(),
            elementTypeInfo,
            rules.ToEquatableImmutableArray());
    }
}
