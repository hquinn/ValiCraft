using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotEmpty")]
[DefaultMessage("{TargetName} cannot be empty.")]
public class NotEmpty<TTargetType> : IValidationRule<TTargetType?>
{
    public static bool IsValid(TTargetType? property)
    {
        return EqualityComparer<TTargetType>.Default.Equals(property, default);
    }
}