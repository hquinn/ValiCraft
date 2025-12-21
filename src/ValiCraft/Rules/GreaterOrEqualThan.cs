using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is greater than or equal to another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
/// </remarks>
[GenerateRuleExtension("IsGreaterOrEqualThan")]
[DefaultMessage("{TargetName} must be greater or equal than {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class GreaterOrEqualThan<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IComparable
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.CompareTo(parameter) >= 0;
    }
}
