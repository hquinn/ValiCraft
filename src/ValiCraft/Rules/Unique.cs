using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsUnique")]
[DefaultMessage("{TargetName} must contain only unique items")]
public class Unique<TTargetType> : IValidationRule<IEnumerable<TTargetType>?>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue)
    {
        if (targetValue == null) return true;
        var list = targetValue.ToList();
        return list.Count == list.Distinct().Count();
    }
}
