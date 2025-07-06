using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsGreaterOrEqualThan")]
[DefaultMessage("{PropertyName} must be greater or equal than {ValueToCompare}. Value received is {PropertyValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class GreaterOrEqualThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) >= 0;
    }
}