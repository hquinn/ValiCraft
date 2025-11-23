using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("CollectionNotContains")]
[DefaultMessage("{TargetName} must not contain the specified item")]
[RulePlaceholder("{Item}", "parameter")]
public class CollectionNotContains<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
    {
        if (targetValue == null) return true;
        return !targetValue.Contains(parameter);
    }
}
