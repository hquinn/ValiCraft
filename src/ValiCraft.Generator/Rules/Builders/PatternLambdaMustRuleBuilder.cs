using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class PatternLambdaMustRuleBuilder(
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static PatternLambdaMustRuleBuilder Create(
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new PatternLambdaMustRuleBuilder(
            rewrittenBody.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new PatternLambdaMustRule(
            expressionFormat,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            GetRuleOverrideData(),
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}