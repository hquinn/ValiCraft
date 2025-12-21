using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the past (before the current UTC time).
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsInPast")]
[DefaultMessage("{TargetName} must be in the past")]
public class InPast : IValidationRule<DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue < DateTime.UtcNow;
    }
}
