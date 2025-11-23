using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("CollectionContains")]
[DefaultMessage("{TargetName} must contain the specified item")]
[RulePlaceholder("{Item}", "item")]
[RulePlaceholder("{Comparer}", "comparer")]
public class CollectionContainsWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null) return false;
        return targetValue.Contains(item, comparer);
    }
}
