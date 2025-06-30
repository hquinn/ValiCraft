using ValiCraft.Abstractions;
using ValiCraft.Abstractions.Attributes;

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