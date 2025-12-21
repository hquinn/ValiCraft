using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string is not null or empty.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsNotNullOrEmpty")]
[DefaultMessage("{TargetName} must not be null or empty.")]
public class NotNullOrEmpty : IValidationRule<string?>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue)
    {
        return !string.IsNullOrEmpty(targetValue);
    }
}