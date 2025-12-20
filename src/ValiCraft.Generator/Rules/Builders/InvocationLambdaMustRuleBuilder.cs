using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class InvocationLambdaMustRuleBuilder(
    bool isAsync,
    string body,
    LocationInfo location) : RuleBuilder
{
    public static InvocationLambdaMustRuleBuilder Create(
        bool isAsync,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = isAsync ? lambda.GetSecondParameterName() : null;
        
        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new InvocationLambdaMustRuleBuilder(
            isAsync,
            rewrittenBody.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new InvocationLambdaMustRule(
            isAsync,
            body,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            new MessageInfo(KnownNames.Targets.GetMustTarget(isAsync), true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}