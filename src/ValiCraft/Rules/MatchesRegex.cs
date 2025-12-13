using System.Text.RegularExpressions;
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string matches a specified compiled regular expression.
/// This is more efficient than the string pattern version as the Regex is pre-compiled.
/// </summary>
[GenerateRuleExtension("MatchesRegex")]
[DefaultMessage("{TargetName} must match the specified pattern")]
public class MatchesRegex : IValidationRule<string?, Regex>
{
    public static bool IsValid(string? targetValue, Regex regex)
    {
        if (targetValue == null)
        {
            return false;
        }

        return regex.IsMatch(targetValue);
    }
}
