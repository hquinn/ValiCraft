using ValiCraft;

namespace ValiCraft.Rules;

public class NotNull<T> : IValidationRule<T?>
{
    public static bool IsValid(T? propertyValue)
    {
        return propertyValue is not null;
    }
}