using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not null.
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated.</typeparam>
[GenerateRuleExtension("IsNotNull")]
[DefaultMessage("{TargetName} is required.")]
public class NotNull<TTargetType> : IValidationRule<TTargetType?>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType? targetValue)
    {
        return targetValue is not null;
    }
}