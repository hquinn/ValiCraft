using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class RichSemanticValidationRuleBuilder(
    string methodName,
    EquatableArray<ArgumentInfo> arguments,
    MapToValidationRuleData? mapData,
    MessageInfo? defaultMessage,
    MessageInfo? defaultErrorCode,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    LocationInfo location) : RuleBuilder
{
    public static RichSemanticValidationRuleBuilder Create(
        IMethodSymbol methodSymbol,
        InvocationExpressionSyntax invocation,
        string methodName,
        SemanticModel semanticModel)
    {
        var containingType = methodSymbol.ContainingType;

        return new RichSemanticValidationRuleBuilder(
            methodName,
            invocation.GetArguments(methodSymbol, semanticModel).ToEquatableImmutableArray(),
            MapToValidationRuleData.CreateFromMethodAndAttribute(
                methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute),
            MessageInfo.CreateFromAttribute(containingType, KnownNames.Attributes.DefaultMessageAttribute),
            MessageInfo.CreateFromAttribute(containingType, KnownNames.Attributes.DefaultErrorCodeAttribute),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(containingType),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new RichSemanticValidationRule(
            methodName,
            mapData,
            arguments,
            defaultMessage,
            defaultErrorCode,
            GetRuleOverrideData(),
            rulePlaceholders,
            location);
    }
}