using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class BlockLambdaMustRuleBuilder(
    string body,
    string parameterName,
    LocationInfo location) : RuleBuilder
{
    public static BlockLambdaMustRuleBuilder Create(
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();

        return new BlockLambdaMustRuleBuilder(
            lambda.Body.ToString(),
            parameterName,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new BlockLambdaMustRule(
            body,
            parameterName,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            new MessageInfo("Must", true),
            GetRuleOverrideData(),
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}