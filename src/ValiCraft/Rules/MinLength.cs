using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string has at least a minimum length.
/// </summary>
[GenerateRuleExtension("HasMinLength")]
[DefaultMessage("{TargetName} must have a minimum length of {MinLength}")]
[RulePlaceholder("{MinLength}", "parameter")]
public class MinLength : IValidationRule<string?, int>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, int parameter)
    {
        return targetValue?.Length >= parameter;
    }
}
