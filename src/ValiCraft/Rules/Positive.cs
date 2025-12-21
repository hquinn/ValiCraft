using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is positive (greater than zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
[GenerateRuleExtension("IsPositive")]
[DefaultMessage("{TargetName} must be positive. Value received is {TargetValue}")]
public class Positive<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) > 0;
    }
}
