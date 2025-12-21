using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string ends with a specified suffix.
/// </summary>
[GenerateRuleExtension("EndsWith")]
[DefaultMessage("{TargetName} must end with {Suffix}")]
[RulePlaceholder("{Suffix}", "parameter")]
public class EndsWith : IValidationRule<string?, string>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.EndsWith(parameter);
    }
}
