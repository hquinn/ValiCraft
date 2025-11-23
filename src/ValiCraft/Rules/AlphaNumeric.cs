using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string contains only alphanumeric characters (letters and digits).
/// </summary>
[GenerateRuleExtension("IsAlphaNumeric")]
[DefaultMessage("{TargetName} must contain only letters and numbers")]
public class AlphaNumeric : IValidationRule<string?>
{
    public static bool IsValid(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        return targetValue.All(char.IsLetterOrDigit);
    }
}
