using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNull")]
[DefaultMessage("{TargetName} is required.")]
public class NotNull<TPropertyType> : IValidationRule<TPropertyType?>
{
    public static bool IsValid(TPropertyType? targetValue)
    {
        return targetValue is not null;
    }
}