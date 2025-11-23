using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("EndsWith")]
[DefaultMessage("{TargetName} must end with {Suffix}")]
[RulePlaceholder("{Suffix}", "parameter")]
public class EndsWith : IValidationRule<string?, string>
{
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null) return false;
        return targetValue.EndsWith(parameter);
    }
}
