using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class BlockLambdaRuleBuilder(
    bool isAsync,
    string body,
    string parameterName,
    string? cancellationTokenParameterName,
    LocationInfo location) : RuleBuilder
{
    public static BlockLambdaRuleBuilder Create(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;

        return new BlockLambdaRuleBuilder(
            isAsync,
            lambda.Body.ToString(),
            parameterName,
            cancellationTokenParameterName,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new BlockLambdaRule(
            isAsync,
            body,
            parameterName,
            cancellationTokenParameterName,
            null,
            new MessageInfo(KnownNames.Targets.Is, true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}
