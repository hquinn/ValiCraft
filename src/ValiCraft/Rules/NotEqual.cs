using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotEqualTo")]
[DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class NotEqual<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return !property.Equals(parameter);
    }
}