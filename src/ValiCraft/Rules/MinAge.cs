using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a birth date represents a person who is at least a specified minimum age in years.
/// </summary>
[GenerateRuleExtension("HasMinAge")]
[DefaultMessage("{TargetName} must represent an age of at least {MinAge} years")]
[RulePlaceholder("{MinAge}", "parameter")]
public class MinAge : IValidationRule<DateTime, int>
{
    public static bool IsValid(DateTime birthDate, int parameter)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }
        return age >= parameter;
    }
}
