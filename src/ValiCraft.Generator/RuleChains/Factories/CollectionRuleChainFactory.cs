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
        ValidationTarget @object,
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        var lambdaInfo = invocation.GetLambdaInfoFromLastArgument();

        if (!LambdaInfo.IsValid(lambdaInfo, invocation, KnownNames.Methods.EnsureEach, diagnostics))
        {
            return null;
        }
        
        var ruleChains = new List<RuleChain>();
        var elementDepth = depth + 1;
        var elementIndent = IndentModel.CreateChild(indent);

        foreach (var statement in lambdaInfo!.Statements)
        {
            var ruleChain = RuleChainFactory.CreateFromStatement(
                statement,
                lambdaInfo.ParameterName!,
                elementDepth,
                elementIndent,
                diagnostics,
                context);

            if (ruleChain is not null)
            {
                ruleChains.Add(ruleChain);
            }
        }

        // If we don't have any rule chains in the collection, then don't bother
        if (ruleChains.Count == 0)
        {
            return null;
        }
        
        return new CollectionRuleChain(
            @object,
            target!,
            depth,
            indent,
            ruleChains.Sum(x => x.NumberOfRules),
            invocation.GetOnFailureModeFromSyntax(),
            ruleChains.ToEquatableImmutableArray());
    }
}