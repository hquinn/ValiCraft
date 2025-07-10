using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotEmpty")]
[DefaultMessage("{TargetName} cannot be empty.")]
public class NotEmpty<TPropertyType> : IValidationRule<TPropertyType?>
{
    public static bool IsValid(TPropertyType? property)
    {
        return EqualityComparer<TPropertyType>.Default.Equals(property, default);
    }
}