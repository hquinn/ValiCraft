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
    public static bool IsValid(IEnumerable<TTargetType>? targetValue)
    {
        return targetValue == null || !targetValue.Any();
    }
}
