using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasLength")]
[DefaultMessage("{TargetName} must have exactly {ExpectedLength} characters. Current length is {CurrentLength}")]
[RulePlaceholder("{ExpectedLength}", "parameter")]
public class Length : IValidationRule<string?, int>
{
    public static bool IsValid(string? targetValue, int parameter)
    {
        return targetValue?.Length == parameter;
    }
}
