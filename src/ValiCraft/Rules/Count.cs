using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection has exactly a specified number of items.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection</typeparam>
[GenerateRuleExtension("HasCount")]
[DefaultMessage("{TargetName} must have exactly {ExpectedCount} items")]
[RulePlaceholder("{ExpectedCount}", "parameter")]
public class Count<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
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
            return collection.Count == parameter;
        }

        return targetValue.Count() == parameter;
    }
}
