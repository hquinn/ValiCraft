using ValiCraft.Abstractions;

namespace ValiCraft.Rules;

public class NotEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? propertyValue)
    {
        return !string.IsNullOrEmpty(propertyValue);
    }
}