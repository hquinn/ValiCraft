using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNull")]
[DefaultMessage("{PropertyName} is required.")]
public class NotNull<T> : IValidationRule<T?>
{
    public static bool IsValid(T? propertyValue)
    {
        return propertyValue is not null;
    }
}