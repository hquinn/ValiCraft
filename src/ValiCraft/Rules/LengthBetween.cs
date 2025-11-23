using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasLengthBetween")]
[DefaultMessage("{TargetName} must have a length between {MinLength} and {MaxLength}. Current length is {CurrentLength}")]
[RulePlaceholder("{MinLength}", "minLength")]
[RulePlaceholder("{MaxLength}", "maxLength")]
public class LengthBetween : IValidationRule<string?, int, int>
{
    public static bool IsValid(string? targetValue, int minLength, int maxLength)
    {
        if (targetValue == null) return false;
        var length = targetValue.Length;
        return length >= minLength && length <= maxLength;
    }
}
