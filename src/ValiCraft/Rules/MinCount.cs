using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection has at least a minimum number of items.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection</typeparam>
[GenerateRuleExtension("HasMinCount")]
[DefaultMessage("{TargetName} must have at least {MinCount} items")]
[RulePlaceholder("{MinCount}", "parameter")]
public class MinCount<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count >= parameter;
        }

        return targetValue.Count() >= parameter;
    }
}
