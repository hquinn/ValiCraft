using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a string starts with a specified prefix.
/// </summary>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Prefix}</c>.
/// </remarks>
[GenerateRuleExtension("StartsWith")]
[DefaultMessage("{TargetName} must start with {Prefix}")]
[RulePlaceholder("{Prefix}", "parameter")]
public class StartsWith : IValidationRule<string?, string>
{
    /// <inheritdoc />
    public static bool IsValid(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.StartsWith(parameter, StringComparison.Ordinal);
    }
}
