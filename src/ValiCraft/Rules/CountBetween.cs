using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasCountBetween")]
[DefaultMessage("{TargetName} must have between {MinCount} and {MaxCount} items. Current count is {CurrentCount}")]
[RulePlaceholder("{MinCount}", "minCount")]
[RulePlaceholder("{MaxCount}", "maxCount")]
public class CountBetween<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int minCount, int maxCount)
    {
        if (targetValue == null) return false;
        var count = targetValue.Count();
        return count >= minCount && count <= maxCount;
    }
}
