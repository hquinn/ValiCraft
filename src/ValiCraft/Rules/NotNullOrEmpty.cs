using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNullOrEmpty")]
[DefaultMessage("{TargetName} is must be not null or empty.")]
public class NotNullOrEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? targetValue)
    {
        return !string.IsNullOrEmpty(targetValue);
    }
}