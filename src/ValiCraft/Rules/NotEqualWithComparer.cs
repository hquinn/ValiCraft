using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not equal to another specified value using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared.</typeparam>
[GenerateRuleExtension("IsNotEqualTo")]
[DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "value")]
[RulePlaceholder("{Comparer}", "comparer")]
public class NotEqualWithComparer<TTargetType> : IValidationRule<TTargetType, TTargetType, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return !comparer.Equals(property, value);
    }
}
