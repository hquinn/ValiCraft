using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsEqualTo")]
[DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "value")]
[RulePlaceholder("{Comparer}", "comparer")]
public class EqualWithComparer<TTargetType> : IValidationRule<TTargetType, TTargetType, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return comparer.Equals(property, value);
    }
}
