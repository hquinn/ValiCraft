using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsPositiveOrZero")]
[DefaultMessage("{TargetName} must be positive or zero. Value received is {TargetValue}")]
public class PositiveOrZero<TTargetType> : IValidationRule<TTargetType>
    where TTargetType : IComparable
{
    public static bool IsValid(TTargetType targetValue)
    {
        return targetValue.CompareTo(default(TTargetType)) >= 0;
    }
}
