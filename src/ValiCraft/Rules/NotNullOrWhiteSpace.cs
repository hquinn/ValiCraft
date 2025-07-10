using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNullOrWhiteSpace")]
[DefaultMessage("{TargetName} is must be not null or contain only white space.")]
public class NotNullOrWhiteSpace : IValidationRule<string?>
{
    public static bool IsValid(string? targetValue)
    {
        return !string.IsNullOrWhiteSpace(targetValue);
    }
}