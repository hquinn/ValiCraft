using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasMaxLength")]
[DefaultMessage("{TargetName} must have a maximum length of {MaxLength}. Current length is {CurrentLength}")]
[RulePlaceholder("{MaxLength}", "parameter")]
public class MaxLength : IValidationRule<string?, int>
{
    public static bool IsValid(string? targetValue, int parameter)
    {
        return targetValue?.Length <= parameter;
    }
}
