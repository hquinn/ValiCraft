using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNull")]
[DefaultMessage("{TargetName} is required.")]
public class NotNull<TTargetType> : IValidationRule<TTargetType?>
{
    public static bool IsValid(TTargetType? targetValue)
    {
        return targetValue is not null;
    }
}