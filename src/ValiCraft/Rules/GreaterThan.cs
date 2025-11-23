using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is greater than another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsGreaterThan")]
[DefaultMessage("{TargetName} must be greater than {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class GreaterThan<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.CompareTo(parameter) > 0;
    }
}
