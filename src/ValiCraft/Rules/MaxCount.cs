using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasMaxCount")]
[DefaultMessage("{TargetName} must have at most {MaxCount} items. Current count is {CurrentCount}")]
[RulePlaceholder("{MaxCount}", "parameter")]
public class MaxCount<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        return targetValue?.Count() <= parameter;
    }
}
