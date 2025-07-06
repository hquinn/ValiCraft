using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsEqualTo")]
[DefaultMessage("{PropertyName} must be equal to {ValueToCompare}. Value received is {PropertyValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class Equal<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) == 0;
    }
}