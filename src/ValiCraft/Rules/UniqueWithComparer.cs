using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a collection contains only unique items (no duplicates) using a custom equality comparer.
/// </summary>
/// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
/// <remarks>
/// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Comparer}</c>.
/// </remarks>
[GenerateRuleExtension("IsUnique")]
[DefaultMessage("{TargetName} must contain only unique items")]
[RulePlaceholder("{Comparer}", "comparer")]
public class UniqueWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, IEqualityComparer<TTargetType>>
{
    /// <inheritdoc />
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return true;
        }

        var seen = new HashSet<TTargetType>(comparer);
        foreach (var item in targetValue)
        {
            if (!seen.Add(item))
            {
                return false;
            }
        }

        return true;
    }
}
