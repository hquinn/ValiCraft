using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsEmpty")]
[DefaultMessage("{TargetName} must be empty")]
public class Empty<TTargetType> : IValidationRule<IEnumerable<TTargetType>?>
{
    public static bool IsValid(IEnumerable<TTargetType>? targetValue)
    {
        return targetValue == null || !targetValue.Any();
    }
}
