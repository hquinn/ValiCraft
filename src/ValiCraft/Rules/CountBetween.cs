using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection has a number of items within a specified range.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection</typeparam>
[GenerateRuleExtension("HasCountBetween")]
[DefaultMessage("{TargetName} must have between {MinCount} and {MaxCount} items")]
[RulePlaceholder("{MinCount}", "minCount")]
[RulePlaceholder("{MaxCount}", "maxCount")]
public class CountBetween<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int minCount, int maxCount)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        int count;
        if (targetValue is ICollection<TTargetType> collection)
        {
            count = collection.Count;
        }
        else
        {
            count = targetValue.Count();
        }

        return count >= minCount && count <= maxCount;
    }
}
