using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record RuleInvocation(
    ArgumentInfo Property,
    string MethodName,
    EquatableArray<ArgumentInfo> Arguments,
    MapToValidationRuleData? ValidationRuleData,
    RuleOverrideData RuleOverrides,
    MessageInfo? DefaultMessage,
    EquatableArray<RulePlaceholderInfo> Placeholders);