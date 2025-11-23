using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNegativeOrZero")]
[DefaultMessage("{TargetName} must be negative or zero. Value received is {TargetValue}")]
public class NegativeOrZero<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) <= 0;
    }
}
