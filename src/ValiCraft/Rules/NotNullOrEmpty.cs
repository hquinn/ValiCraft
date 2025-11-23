using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string is not null or empty.
/// </summary>
[GenerateRuleExtension("IsNotNullOrEmpty")]
[DefaultMessage("{TargetName} is must be not null or empty.")]
public class NotNullOrEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? targetValue)
    {
        return !string.IsNullOrEmpty(targetValue);
    }
}