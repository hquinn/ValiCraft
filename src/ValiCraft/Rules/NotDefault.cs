using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not the default value for its type.
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsNotDefault")]
[DefaultMessage("{TargetName} cannot be the default value.")]
public class NotDefault<TTargetType> : IValidationRule<TTargetType?>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType? property)
    {
        return !EqualityComparer<TTargetType>.Default.Equals(property, default);
    }
}