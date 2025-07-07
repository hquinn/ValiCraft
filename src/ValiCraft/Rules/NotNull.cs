using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNull")]
[DefaultMessage("{PropertyName} is required.")]
public class NotNull<TPropertyType> : IValidationRule<TPropertyType?>
{
    public static bool IsValid(TPropertyType? propertyValue)
    {
        return propertyValue is not null;
    }
}