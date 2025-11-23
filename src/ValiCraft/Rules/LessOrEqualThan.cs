using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is less than or equal to another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsLessOrEqualThan")]
[DefaultMessage("{TargetName} must be less or equal than {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class LessOrEqualThan<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.CompareTo(parameter) <= 0;
    }
}
