using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsUnique")]
[DefaultMessage("{TargetName} must contain only unique items")]
[RulePlaceholder("{Comparer}", "comparer")]
public class UniqueWithComparer<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, IEqualityComparer<TTargetType>>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null) return true;
        var list = targetValue.ToList();
        return list.Count == list.Distinct(comparer).Count();
    }
}
