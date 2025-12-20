using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class BlockLambdaMustRuleBuilder(
    bool isAsync,
    string body,
    string parameterName,
    string? cancellationTokenParameterName,
    LocationInfo location) : RuleBuilder
{
    public static BlockLambdaMustRuleBuilder Create(
        bool isAsync,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = lambda.GetSecondParameterName();

        return new BlockLambdaMustRuleBuilder(
            isAsync,
            lambda.Body.ToString(),
            parameterName,
            cancellationTokenParameterName,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new BlockLambdaMustRule(
            isAsync,
            body,
            parameterName,
            cancellationTokenParameterName,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            new MessageInfo(KnownNames.Targets.GetMustTarget(isAsync), true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}