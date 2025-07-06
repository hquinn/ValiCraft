using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsLessOrEqualThan")]
[DefaultMessage("{PropertyName} must be less or equal than {ValueToCompare}. Value received is {PropertyValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class LessOrEqualThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) <= 0;
    }
}