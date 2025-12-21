using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not equal to another specified value using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>, <c>{Comparer}</c>.
/// </remarks>
[GenerateRuleExtension("IsNotEqualTo")]
[DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "value")]
[RulePlaceholder("{Comparer}", "comparer")]
public class NotEqualWithComparer<TTargetType> : IValidationRule<TTargetType, TTargetType, IEqualityComparer<TTargetType>>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return !comparer.Equals(property, value);
    }
}
