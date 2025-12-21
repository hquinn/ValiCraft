using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string has exactly a specified length.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ExpectedLength}</c>.
/// </remarks>
[GenerateRuleExtension("HasLength")]
[DefaultMessage("{TargetName} must have exactly {ExpectedLength} characters")]
[RulePlaceholder("{ExpectedLength}", "parameter")]
public class Length : IValidationRule<string?, int>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, int parameter)
    {
        return targetValue?.Length == parameter;
    }
}
