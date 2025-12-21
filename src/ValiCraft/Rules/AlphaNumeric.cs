using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string contains only alphanumeric characters (letters and digits).
/// </summary>
[GenerateRuleExtension("IsAlphaNumeric")]
[DefaultMessage("{TargetName} must contain only letters and numbers")]
public class AlphaNumeric : IValidationRule<string?>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        foreach (var c in targetValue)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}
