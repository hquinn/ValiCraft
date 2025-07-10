using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotEqualTo")]
[DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class NotEqual<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IEquatable<TPropertyType>
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return !property.Equals(parameter);
    }
}