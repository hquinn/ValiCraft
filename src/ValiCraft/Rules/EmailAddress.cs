using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string is a valid email address format.
/// Performs a simple check for presence of @ with text before and after.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsEmailAddress")]
[DefaultMessage("{TargetName} must be a valid email address")]
public class EmailAddress : IValidationRule<string?>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        var atIndex = targetValue.IndexOf('@');

        // Must contain @ with at least one character before and after
        return atIndex > 0 && atIndex < targetValue.Length - 1;
    }
}
