using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CompositeRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        var onFailureArgument = invocation?.GetOnFailureModeFromSyntax();

        if (onFailureArgument is null)
        {
            return null;
        }
        
        var lambdaInfo = invocation!.GetLambdaInfoFromLastArgument();

        if (!LambdaInfo.IsValid(lambdaInfo, invocation!, KnownNames.Methods.EnsureEach, diagnostics))
        {
            return null;
        }
        
        var ruleChains = new List<RuleChain>();

        foreach (var statement in lambdaInfo!.Statements)
        {
            var ruleChain = RuleChainFactory.CreateFromStatement(
                statement,
                lambdaInfo.ParameterName!,
                depth,
                diagnostics,
                context);

            if (ruleChain is not null)
            {
                ruleChains.Add(ruleChain);
            }
        }

        // If we don't have any rule chains, then don't bother
        if (ruleChains.Count == 0)
        {
            return null;
        }
        
        return new CompositeRuleChain(
            depth,
            ruleChains.Sum(x => x.NumberOfRules),
            onFailureArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}