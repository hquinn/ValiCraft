using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is between a start date and end date (inclusive).
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{StartDate}</c>, <c>{EndDate}</c>.
/// </remarks>
[GenerateRuleExtension("IsDateBetween")]
[DefaultMessage("{TargetName} must be between {StartDate} and {EndDate}")]
[RulePlaceholder("{StartDate}", "startDate")]
[RulePlaceholder("{EndDate}", "endDate")]
public class DateBetween : IValidationRule<DateTime, DateTime, DateTime>
{
    /// <inheritdoc />
    public static bool IsValid(DateTime targetValue, DateTime startDate, DateTime endDate)
    {
        return targetValue >= startDate && targetValue <= endDate;
    }
}
