using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNegative")]
[DefaultMessage("{TargetName} must be negative. Value received is {TargetValue}")]
public class Negative<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) < 0;
    }
}
