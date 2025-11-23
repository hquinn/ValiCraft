using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasMinLength")]
[DefaultMessage("{TargetName} must have a minimum length of {MinLength}. Current length is {CurrentLength}")]
[RulePlaceholder("{MinLength}", "parameter")]
public class MinLength : IValidationRule<string?, int>
{
    public static bool IsValid(string? targetValue, int parameter)
    {
        return targetValue?.Length >= parameter;
    }
}
