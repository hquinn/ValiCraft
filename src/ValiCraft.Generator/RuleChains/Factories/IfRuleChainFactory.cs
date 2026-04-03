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
        var ifConditionArgument = IfConditionFactory.Create(invocation, true);

        // No need for diagnostics, as having a null condition in this case isn't legal C#
        if (ifConditionArgument is null)
        {
            return null;
        }
        
        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            isAsyncValidator, invocation, KnownNames.Methods.If,
            depth, IndentModel.CreateChild(indent), diagnostics, context);
        if (ruleChains is null)
        {
            return null;
        }

        return new IfRuleChain(
            isAsyncValidator,
            @object,
            depth,
            indent,
            ruleChains.Sum(x => x.NumberOfRules),
            ifConditionArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}