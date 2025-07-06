using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsGreaterThan")]
[DefaultMessage("{PropertyName} must be greater than {ValueToCompare}. Value received is {PropertyValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class GreaterThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) > 0;
    }
}