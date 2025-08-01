using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class IdentifierNameMustRuleBuilder(
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static IdentifierNameMustRuleBuilder Create(
        InvocationExpressionSyntax invocation,
        IdentifierNameSyntax identifierNameSyntax)
    {
        var expressionFormat = $"{identifierNameSyntax.Identifier.ValueText}({{0}})";

        return new IdentifierNameMustRuleBuilder(
            expressionFormat,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new IdentifierNameMustRule(
            expressionFormat,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            new MessageInfo("Must", true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}