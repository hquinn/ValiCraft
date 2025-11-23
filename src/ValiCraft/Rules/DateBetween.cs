using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsDateBetween")]
[DefaultMessage("{TargetName} must be between {StartDate} and {EndDate}")]
[RulePlaceholder("{StartDate}", "startDate")]
[RulePlaceholder("{EndDate}", "endDate")]
public class DateBetween : IValidationRule<DateTime, DateTime, DateTime>
{
    public static bool IsValid(DateTime targetValue, DateTime startDate, DateTime endDate)
    {
        return targetValue >= startDate && targetValue <= endDate;
    }
}
