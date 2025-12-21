using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a birth date represents a person who is at most a specified maximum age in years.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxAge}</c>.
/// </remarks>
[GenerateRuleExtension("HasMaxAge")]
[DefaultMessage("{TargetName} must represent an age of at most {MaxAge} years")]
[RulePlaceholder("{MaxAge}", "parameter")]
public class MaxAge : IValidationRule<DateTime, int>
{
    /// <inheritdoc />
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
