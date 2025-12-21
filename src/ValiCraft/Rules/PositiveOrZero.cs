using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is positive or zero (greater than or equal to zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
[GenerateRuleExtension("IsPositiveOrZero")]
[DefaultMessage("{TargetName} must be positive or zero. Value received is {TargetValue}")]
public class PositiveOrZero<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default!) >= 0;
    }
}
