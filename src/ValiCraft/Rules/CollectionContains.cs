using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection contains a specified item.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection. Must implement IEquatable.</typeparam>
[GenerateRuleExtension("CollectionContains")]
[DefaultMessage("{TargetName} must contain the specified item")]
[RulePlaceholder("{Item}", "parameter")]
public class CollectionContains<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(parameter);
    }
}
