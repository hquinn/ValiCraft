using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection is empty or null.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
[GenerateRuleExtension("IsEmpty")]
[DefaultMessage("{TargetName} must be empty")]
public class Empty<TTargetType> : IValidationRule<IEnumerable<TTargetType>?>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue)
    {
        if (targetValue == null)
        {
            return true;
        }

        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count == 0;
        }

        if (targetValue is IReadOnlyCollection<TTargetType> readOnlyCollection)
        {
            return readOnlyCollection.Count == 0;
        }

        return !targetValue.Any();
    }
}
