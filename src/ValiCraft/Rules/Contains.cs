using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("Contains")]
[DefaultMessage("{TargetName} must contain {Substring}")]
[RulePlaceholder("{Substring}", "parameter")]
public class Contains : IValidationRule<string?, string>
{
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null) return false;
        return targetValue.Contains(parameter);
    }
}
