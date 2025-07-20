using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class IfRuleChainFactory : IRuleChainFactory
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
        var ifConditionArgument = IfConditionFactory.Create(invocation, true);

        // No need for diagnostics, as having a null condition in this case isn't legal C#
        if (ifConditionArgument is null)
        {
            return null;
        }
        
        var lambdaInfo = invocation.GetLambdaInfoFromLastArgument();

        if (!LambdaInfo.IsValid(lambdaInfo, invocation, KnownNames.Methods.If, diagnostics))
        {
            return null;
        }
        
        var ruleChains = new List<RuleChain>();
        var childIndent = IndentModel.CreateChild(indent);

        foreach (var statement in lambdaInfo!.Statements)
        {
            var ruleChain = RuleChainFactory.CreateFromStatement(
                statement,
                lambdaInfo.ParameterName!,
                depth,
                childIndent,
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
        
        return new IfRuleChain(
            @object,
            depth,
            indent,
            ruleChains.Sum(x => x.NumberOfRules),
            ifConditionArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}