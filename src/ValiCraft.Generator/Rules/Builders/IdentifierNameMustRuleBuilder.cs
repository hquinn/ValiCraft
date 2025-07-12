using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class IdentifierNameMustRuleBuilder(
    string body,
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
            body,
            new MessageInfo("'{TargetName}' doesn't satisfy the condition", true),
            GetRuleOverrideData(),
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}