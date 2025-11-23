using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string has a length within a specified range.
/// </summary>
[GenerateRuleExtension("HasLengthBetween")]
[DefaultMessage("{TargetName} must have a length between {MinLength} and {MaxLength}")]
[RulePlaceholder("{MinLength}", "minLength")]
[RulePlaceholder("{MaxLength}", "maxLength")]
public class LengthBetween : IValidationRule<string?, int, int>
{
    public static bool IsValid(string? targetValue, int minLength, int maxLength)
    {
        if (targetValue == null)
        {
            return false;
        }

        var length = targetValue.Length;
        return length >= minLength && length <= maxLength;
    }
}
