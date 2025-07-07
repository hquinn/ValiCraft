using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotNullOrEmpty")]
[DefaultMessage("{PropertyName} is must be not null or empty.")]
public class NotNullOrEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? propertyValue)
    {
        return !string.IsNullOrEmpty(propertyValue);
    }
}