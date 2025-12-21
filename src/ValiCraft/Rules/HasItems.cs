using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection is not null and contains at least one item.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
[GenerateRuleExtension("HasItems")]
[DefaultMessage("{TargetName} cannot be empty.")]
public class HasItems<TTargetType> : IValidationRule<IEnumerable<TTargetType>?>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? property)
    {
        if (property == null)
        {
            return false;
        }

        if (property is ICollection<TTargetType> collection)
        {
            return collection.Count > 0;
        }

        if (property is IReadOnlyCollection<TTargetType> readOnlyCollection)
        {
            return readOnlyCollection.Count > 0;
        }

        return property.Any();
    }
}