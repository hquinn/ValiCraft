using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("HasItems")]
[DefaultMessage("{TargetName} cannot be empty.")]
public class HasItems<TTargetType> : IValidationRule<IEnumerable<TTargetType>?>
{
    public static bool IsValid(IEnumerable<TTargetType>? property)
    {
        return property?.Any() ?? false;
    }
}