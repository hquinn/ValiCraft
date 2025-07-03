using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsGreaterThan")]
[DefaultMessage("{PropertyName} must be greater than {ArgumentValue}. Value received is {PropertyValue}")]
[RulePlaceholder("{ArgumentValue}", "parameter")]
public class GreaterThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) > 0;
    }
}