using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class PatternLambdaMustRuleBuilder(
    bool isAsync,
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static PatternLambdaMustRuleBuilder Create(
        bool isAsync,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = isAsync ? lambda.GetSecondParameterName() : null;

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new PatternLambdaMustRuleBuilder(
            isAsync,
            rewrittenBody.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new PatternLambdaMustRule(
            isAsync,
            expressionFormat,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            new MessageInfo(KnownNames.Targets.GetMustTarget(isAsync), true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}