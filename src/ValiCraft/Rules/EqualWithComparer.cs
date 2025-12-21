using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is equal to another specified value using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared.</typeparam>
[GenerateRuleExtension("IsEqualTo")]
[DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "value")]
[RulePlaceholder("{Comparer}", "comparer")]
public class EqualWithComparer<TTargetType> : IValidationRule<TTargetType, TTargetType, IEqualityComparer<TTargetType>>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return comparer.Equals(property, value);
    }
}
