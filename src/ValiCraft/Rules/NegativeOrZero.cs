using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is negative or zero (less than or equal to zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsNegativeOrZero")]
[DefaultMessage("{TargetName} must be negative or zero. Value received is {TargetValue}")]
public class NegativeOrZero<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default!) <= 0;
    }
}
