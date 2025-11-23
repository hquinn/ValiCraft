using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is greater than or equal to another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsGreaterOrEqualThan")]
[DefaultMessage("{TargetName} must be greater or equal than {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class GreaterOrEqualThan<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.CompareTo(parameter) >= 0;
    }
}
