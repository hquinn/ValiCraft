using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection does not contain a specified item.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection. Must implement IEquatable.</typeparam>
[GenerateRuleExtension("CollectionNotContains")]
[DefaultMessage("{TargetName} must not contain the specified item")]
[RulePlaceholder("{Item}", "parameter")]
public class CollectionNotContains<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
    {
        if (targetValue == null)
        {
            return true;
        }

        return !targetValue.Contains(parameter);
    }
}
