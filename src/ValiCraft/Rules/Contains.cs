using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string contains a specified substring.
/// </summary>
[GenerateRuleExtension("Contains")]
[DefaultMessage("{TargetName} must contain {Substring}")]
[RulePlaceholder("{Substring}", "parameter")]
public class Contains : IValidationRule<string?, string>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(parameter, StringComparison.Ordinal);
    }
}
