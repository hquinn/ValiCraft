using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasMaxAge")]
[DefaultMessage("{TargetName} must represent an age of at most {MaxAge} years")]
[RulePlaceholder("{MaxAge}", "parameter")]
public class MaxAge : IValidationRule<DateTime, int>
{
    public static bool IsValid(DateTime birthDate, int parameter)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }
        return age <= parameter;
    }
}
