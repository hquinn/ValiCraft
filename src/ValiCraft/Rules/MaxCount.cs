using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection has at most a maximum number of items.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxCount}</c>.
/// </remarks>
[GenerateRuleExtension("HasMaxCount")]
[DefaultMessage("{TargetName} must have at most {MaxCount} items")]
[RulePlaceholder("{MaxCount}", "parameter")]
public class MaxCount<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count <= parameter;
        }

        return targetValue.Count() <= parameter;
    }
}
