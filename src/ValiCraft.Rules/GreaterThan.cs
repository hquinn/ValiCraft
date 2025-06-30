using ValiCraft;
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsGreaterThan")]
public class GreaterThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) > 0;
    }
}