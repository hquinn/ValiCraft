using System.Text.RegularExpressions;
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsEmailAddress")]
[DefaultMessage("{TargetName} must be a valid email address")]
public class EmailAddress : IValidationRule<string?>
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    
    public static bool IsValid(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue)) return false;
        return Regex.IsMatch(targetValue, EmailPattern);
    }
}
