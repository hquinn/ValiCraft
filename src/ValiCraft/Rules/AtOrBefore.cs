using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is at or before the specified reference time.
/// This allows for testable date validation by providing a reference point.
/// </summary>
[GenerateRuleExtension("IsAtOrBefore")]
[DefaultMessage("{TargetName} must be at or before {ReferenceDate}")]
[RulePlaceholder("{ReferenceDate}", "referenceDate")]
public class AtOrBefore : IValidationRule<DateTime, DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue <= referenceDate;
    }
}
