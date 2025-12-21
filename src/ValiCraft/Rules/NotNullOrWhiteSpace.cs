using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string is not null, empty, or consists only of white-space characters.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsNotNullOrWhiteSpace")]
[DefaultMessage("{TargetName} is must be not null or contain only white space.")]
public class NotNullOrWhiteSpace : IValidationRule<string?>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue)
    {
        return !string.IsNullOrWhiteSpace(targetValue);
    }
}