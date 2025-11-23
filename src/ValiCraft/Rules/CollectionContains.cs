using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("CollectionContains")]
[DefaultMessage("{TargetName} must contain the specified item")]
[RulePlaceholder("{Item}", "parameter")]
public class CollectionContains<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
    {
        if (targetValue == null) return false;
        return targetValue.Contains(parameter);
    }
}
