using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is at or after the specified reference time.
/// This allows for testable date validation by providing a reference point.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
/// </remarks>
[GenerateRuleExtension("IsAtOrAfter")]
[DefaultMessage("{TargetName} must be at or after {ReferenceDate}")]
[RulePlaceholder("{ReferenceDate}", "referenceDate")]
public class AtOrAfter : IValidationRule<DateTime, DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue >= referenceDate;
    }
}
