using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsEqualTo")]
[DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class Equal<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IEquatable<TPropertyType>
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.Equals(parameter);
    }
}