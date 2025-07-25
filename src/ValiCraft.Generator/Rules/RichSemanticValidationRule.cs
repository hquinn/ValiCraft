using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record RichSemanticValidationRule(
    string MethodName,
    MapToValidationRuleData? ValidationRuleData,
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : InvokedValidationRule(
    MethodName,
    ValidationRuleData,
    Arguments,
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    IfCondition,
    Placeholders,
    Location)
{
    public override Rule? EnrichRule(
        ValidationTarget target,
        ValidationRule[] validRules,
        SourceProductionContext context)
    {
        return this;
    }
}