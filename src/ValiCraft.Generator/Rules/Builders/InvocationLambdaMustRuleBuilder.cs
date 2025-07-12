using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class InvocationLambdaMustRuleBuilder(
    string body,
    LocationInfo location) : RuleBuilder
{
    public static InvocationLambdaMustRuleBuilder Create(
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new InvocationLambdaMustRuleBuilder(
            rewrittenBody.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new InvocationLambdaMustRule(
            body,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            GetRuleOverrideData(),
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}