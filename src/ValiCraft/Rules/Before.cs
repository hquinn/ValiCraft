using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the past (before the specified reference time).
/// This allows for testable date validation by providing a reference point.
/// </summary>
[GenerateRuleExtension("IsBefore")]
[DefaultMessage("{TargetName} must be before {ReferenceDate}")]
[RulePlaceholder("{ReferenceDate}", "referenceDate")]
public class Before : IValidationRule<DateTime, DateTime>
{
    public static bool IsValid(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue < referenceDate;
    }
}
