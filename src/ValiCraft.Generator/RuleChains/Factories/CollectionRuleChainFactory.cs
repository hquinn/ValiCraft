using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionRuleChainFactory : IRuleChainFactory
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
        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            isAsyncValidator, invocation, KnownNames.Methods.EnsureEach,
            depth + 1, IndentModel.CreateChild(indent), diagnostics, context);
        if (ruleChains is null)
        {
            return null;
        }

        return new CollectionRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            ruleChains.Sum(x => x.NumberOfRules),
            invocation.GetOnFailureModeFromSyntax(),
            ruleChains.ToEquatableImmutableArray());
    }
}