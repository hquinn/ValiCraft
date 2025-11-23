using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is negative or zero (less than or equal to zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
[GenerateRuleExtension("IsNegativeOrZero")]
[DefaultMessage("{TargetName} must be negative or zero. Value received is {TargetValue}")]
public class NegativeOrZero<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) <= 0;
    }
}
