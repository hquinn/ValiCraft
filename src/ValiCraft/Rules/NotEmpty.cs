using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsNotEmpty")]
[DefaultMessage("{PropertyName} is required.")]
public class NotEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? propertyValue)
    {
        return !string.IsNullOrEmpty(propertyValue);
    }
}