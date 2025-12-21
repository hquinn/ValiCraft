using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the past or present (at or before the current UTC time).
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsInPastOrPresent")]
[DefaultMessage("{TargetName} must be in the past or present")]
public class InPastOrPresent : IValidationRule<DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue <= DateTime.UtcNow;
    }
}
