using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("Must")]
[DefaultMessage("{PropertyName} doesn't satisfy the condition")]
public class Must<TPropertyType> : IValidationRule<TPropertyType?, Func<TPropertyType?, bool>>
{
    public static bool IsValid(TPropertyType? property, Func<TPropertyType?, bool> predicate)
    {
        return predicate(property);
    }
}