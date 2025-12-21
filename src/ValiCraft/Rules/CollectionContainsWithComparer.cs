using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection contains a specified item using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
[GenerateRuleExtension("CollectionContains")]
[DefaultMessage("{TargetName} must contain the specified item")]
[RulePlaceholder("{Item}", "item")]
[RulePlaceholder("{Comparer}", "comparer")]
public class CollectionContainsWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType, IEqualityComparer<TTargetType>>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(item, comparer);
    }
}
