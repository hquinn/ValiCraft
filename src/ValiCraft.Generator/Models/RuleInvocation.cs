using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record RuleInvocation(
    NameAndTypeInfo Property,
    string MethodName,
    EquatableArray<NameAndTypeInfo> Arguments,
    MapToValidationRuleData? ValidationRuleData,
    RuleOverrideData RuleOverrides,
    MessageInfo? DefaultMessage,
    EquatableArray<RulePlaceholderInfo> Placeholders);