using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasItems")]
[DefaultMessage("{TargetName} cannot be empty.")]
public class HasItems<TPropertyType> : IValidationRule<IEnumerable<TPropertyType>?>
{
    public static bool IsValid(IEnumerable<TPropertyType>? property)
    {
        return property?.Any() ?? false;
    }
}