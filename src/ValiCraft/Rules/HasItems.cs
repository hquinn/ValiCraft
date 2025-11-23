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
    public static bool IsValid(IEnumerable<TTargetType>? property)
    {
        return property?.Any() ?? false;
    }
}