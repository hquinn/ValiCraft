using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection does not contain a specified item using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
[GenerateRuleExtension("CollectionNotContains")]
[DefaultMessage("{TargetName} must not contain the specified item")]
[RulePlaceholder("{Item}", "item")]
[RulePlaceholder("{Comparer}", "comparer")]
public class CollectionNotContainsWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return true;
        }

        return !targetValue.Contains(item, comparer);
    }
}
