using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasCount")]
[DefaultMessage("{TargetName} must have exactly {ExpectedCount} items. Current count is {CurrentCount}")]
[RulePlaceholder("{ExpectedCount}", "parameter")]
public class Count<TTargetType> : IValidationRule<IEnumerable<TTargetType>?, int>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        return targetValue?.Count() == parameter;
    }
}
