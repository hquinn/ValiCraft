using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotDefault")]
[DefaultMessage("{TargetName} cannot be the default value.")]
public class NotDefault<TTargetType> : IValidationRule<TTargetType?>
{
    public static bool IsValid(TTargetType? property)
    {
        return !EqualityComparer<TTargetType>.Default.Equals(property, default);
    }
}