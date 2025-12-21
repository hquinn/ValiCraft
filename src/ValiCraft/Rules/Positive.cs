using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a numeric value is positive (greater than zero).
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
/// </remarks>
[GenerateRuleExtension("IsPositive")]
[DefaultMessage("{TargetName} must be positive. Value received is {TargetValue}")]
public class Positive<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default!) > 0;
    }
}
