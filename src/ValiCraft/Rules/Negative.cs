using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is negative (less than zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
[GenerateRuleExtension("IsNegative")]
[DefaultMessage("{TargetName} must be negative. Value received is {TargetValue}")]
public class Negative<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) < 0;
    }
}
