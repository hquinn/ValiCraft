using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasMinCount")]
[DefaultMessage("{TargetName} must have at least {MinCount} items. Current count is {CurrentCount}")]
[RulePlaceholder("{MinCount}", "parameter")]
public class MinCount<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        return targetValue?.Count() >= parameter;
    }
}
