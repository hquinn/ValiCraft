using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the future or present (at or after the current UTC time).
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsInFutureOrPresent")]
[DefaultMessage("{TargetName} must be in the future or present")]
public class InFutureOrPresent : IValidationRule<DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue >= DateTime.UtcNow;
    }
}
