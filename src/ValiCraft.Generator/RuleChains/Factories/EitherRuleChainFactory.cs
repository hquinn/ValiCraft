using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains.Factories;

public class EitherRuleChainFactory : IRuleChainFactory
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
        var arguments = invocation.ArgumentList.Arguments;
        
        if (arguments.Count < 2)
        {
            return null;
        }

        var ruleSets = new List<EquatableArray<RuleChain>>();
        var childIndent = IndentModel.CreateChild(indent);

        foreach (var argument in arguments)
        {
            var lambdaInfo = GetLambdaInfoFromArgument(argument);
            
            if (!LambdaInfo.IsValid(lambdaInfo, invocation, KnownNames.Methods.Either, diagnostics))
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
                    childIndent,
                    diagnostics,
                    context);

                if (ruleChain is not null)
                {
                    ruleChains.Add(ruleChain);
                }
            }
            
            ruleSets.Add(ruleChains.ToEquatableImmutableArray());
        }

        // Need at least 2 non-empty rule sets
        if (ruleSets.Count(s => s.Count > 0) < 2)
        {
            return null;
        }
        
        return new EitherRuleChain(
            @object,
            depth,
            indent,
            ruleSets.Sum(s => s.Sum(x => x.NumberOfRules)) + 1, // +1 for the Either validation itself
            ruleSets.ToEquatableImmutableArray(),
            null,
            null);
    }

    private static LambdaInfo? GetLambdaInfoFromArgument(ArgumentSyntax argument)
    {
        if (argument.Expression is not LambdaExpressionSyntax lambda)
        {
            return null;
        }

        var parameterName = lambda.GetParameterName();
        
        if (parameterName is null)
        {
            return null;
        }

        var statements = lambda switch
        {
            ParenthesizedLambdaExpressionSyntax { Block: not null } parenLambda => 
                parenLambda.Block.Statements.OfType<ExpressionStatementSyntax>().ToList(),
            SimpleLambdaExpressionSyntax { Block: not null } simpleLambda => 
                simpleLambda.Block.Statements.OfType<ExpressionStatementSyntax>().ToList(),
            _ => new List<ExpressionStatementSyntax>()
        };

        return new LambdaInfo(parameterName, statements);
    }
}
