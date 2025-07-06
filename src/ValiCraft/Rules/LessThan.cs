using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsLessThan")]
[DefaultMessage("{PropertyName} must be less than {ValueToCompare}. Value received is {PropertyValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class LessThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) < 0;
    }
}