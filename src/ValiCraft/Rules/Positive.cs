using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsPositive")]
[DefaultMessage("{TargetName} must be positive. Value received is {TargetValue}")]
public class Positive<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) > 0;
    }
}
