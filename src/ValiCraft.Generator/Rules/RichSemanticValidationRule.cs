using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record RichSemanticValidationRule(
    string MethodName,
    MapToValidationRuleData? ValidationRuleData,
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    RuleOverrideData RuleOverrides,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : InvokedValidationRule(
    MethodName,
    ValidationRuleData,
    Arguments,
    DefaultMessage,
    RuleOverrides,
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