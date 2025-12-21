using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the future (after the specified reference time).
/// This allows for testable date validation by providing a reference point.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
/// </remarks>
[GenerateRuleExtension("IsAfter")]
[DefaultMessage("{TargetName} must be after {ReferenceDate}")]
[RulePlaceholder("{ReferenceDate}", "referenceDate")]
public class After : IValidationRule<DateTime, DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue > referenceDate;
    }
}
