using System.Text.RegularExpressions;
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string matches a specified regular expression pattern.
/// </summary>
/// <remarks>
/// For frequently validated patterns, consider using <see cref="MatchesRegex"/> with a
/// pre-compiled <see cref="Regex"/> instance for better performance.
/// </remarks>
[GenerateRuleExtension("Matches")]
[DefaultMessage("{TargetName} must match the pattern {Pattern}")]
[RulePlaceholder("{Pattern}", "pattern")]
public class Matches : IValidationRule<string?, string>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, string pattern)
    {
        if (targetValue == null)
        {
            return false;
        }

        return Regex.IsMatch(targetValue, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
    }
}
