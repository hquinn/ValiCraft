using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class WeakSemanticValidationRuleBuilder(
    string methodName,
    EquatableArray<ArgumentInfo> arguments,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    LocationInfo location) : RuleBuilder
{
    public static WeakSemanticValidationRuleBuilder Create(
        InvocationExpressionSyntax invocation,
        string methodName,
        SemanticModel semanticModel)
    {
        // We can't provide a lot of information from weak semantics currently, however, this can be considered the discovery phase.
        // We'll be able to (hopefully) add all the necessary information when it comes time later in the pipeline
        // when we have access to the generated validation rules (unique to the weak semantics mode).
        return new WeakSemanticValidationRuleBuilder(
            methodName,
            invocation.GetArguments(null, semanticModel).ToEquatableImmutableArray(),
            EquatableArray<RulePlaceholder>.Empty,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new WeakSemanticValidationRule(
            methodName,
            null,
            arguments,
            null,
            null,
            GetRuleOverrideData(),
            IfCondition,
            rulePlaceholders,
            location);
    }
}