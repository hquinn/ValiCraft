using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("CollectionNotContains")]
[DefaultMessage("{TargetName} must not contain the specified item")]
[RulePlaceholder("{Item}", "item")]
[RulePlaceholder("{Comparer}", "comparer")]
public class CollectionNotContainsWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null) return true;
        return !targetValue.Contains(item, comparer);
    }
}
