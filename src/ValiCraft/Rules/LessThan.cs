using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is less than another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsLessThan")]
[DefaultMessage("{TargetName} must be less than {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class LessThan<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.CompareTo(parameter) < 0;
    }
}
