using System.Text.RegularExpressions;
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("Matches")]
[DefaultMessage("{TargetName} must match the pattern {Pattern}")]
[RulePlaceholder("{Pattern}", "pattern")]
public class Matches : IValidationRule<string?, string>
{
    public static bool IsValid(string? targetValue, string pattern)
    {
        if (targetValue == null) return false;
        return Regex.IsMatch(targetValue, pattern);
    }
}
