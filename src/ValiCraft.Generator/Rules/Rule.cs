using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public enum RuleKind
{
    ExpressionFormat,
    BlockLambda,
    ExtensionMethod
}

public sealed record Rule(
    RuleKind Kind,
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location,
    // ExpressionFormat + BlockLambda variant data
    bool IsAsync = false,
    string? ExpressionFormat = null,
    // BlockLambda variant data
    string? Body = null,
    string? Parameter = null,
    string? CancellationTokenParameter = null,
    // ExtensionMethod variant data
    EquatableArray<string> GenericArguments = default,
    MapToValidationRuleData? ValidationRuleData = null);
