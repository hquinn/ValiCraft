using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsBetweenExclusive")]
[DefaultMessage("{TargetName} must be between {Min} and {Max} (exclusive). Value received is {TargetValue}")]
[RulePlaceholder("{Min}", "min")]
[RulePlaceholder("{Max}", "max")]
public class BetweenExclusive<TTargetType> : IValidationRule<TTargetType, TTargetType, TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue, TTargetType min, TTargetType max)
    {
        return targetValue.CompareTo(min) > 0 && targetValue.CompareTo(max) < 0;
    }
}
