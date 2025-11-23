using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("StartsWith")]
[DefaultMessage("{TargetName} must start with {Prefix}")]
[RulePlaceholder("{Prefix}", "parameter")]
public class StartsWith : IValidationRule<string?, string>
{
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null) return false;
        return targetValue.StartsWith(parameter);
    }
}
