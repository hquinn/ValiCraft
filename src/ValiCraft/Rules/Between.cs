using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is between a minimum and maximum value (inclusive).
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared. Must implement IComparable.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Min}</c>, <c>{Max}</c>.
/// </remarks>
[GenerateRuleExtension("IsBetween")]
[DefaultMessage("{TargetName} must be between {Min} and {Max}. Value received is {TargetValue}")]
[RulePlaceholder("{Min}", "min")]
[RulePlaceholder("{Max}", "max")]
public class Between<TTargetType> : IValidationRule<TTargetType, TTargetType, TTargetType>
    where TTargetType : IComparable
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue, TTargetType min, TTargetType max)
    {
        return targetValue.CompareTo(min) >= 0 && targetValue.CompareTo(max) <= 0;
    }
}
