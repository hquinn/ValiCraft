using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string does not exceed a maximum length.
/// Null strings are considered valid (length 0).
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxLength}</c>.
/// </remarks>
[GenerateRuleExtension("HasMaxLength")]
[DefaultMessage("{TargetName} must have a maximum length of {MaxLength}")]
[RulePlaceholder("{MaxLength}", "parameter")]
public class MaxLength : IValidationRule<string?, int>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, int parameter)
    {
        // Null or empty strings have length 0, which should pass MaxLength validation
        return (targetValue?.Length ?? 0) <= parameter;
    }
}
