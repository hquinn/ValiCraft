using ValiCraft.Abstractions;
using ValiCraft.Abstractions.Attributes;

namespace ValiCraft.Rules;

//[GenerateRuleExtension("IsNotEmpty")]
public class NotEmpty : IValidationRule<string?>
{
    public static bool IsValid(string? propertyValue)
    {
        return !string.IsNullOrEmpty(propertyValue);
    }
}


public static class NotEmptyExtensions
{
    [global::ValiCraft.Abstractions.Attributes.MapToValidationRule(
        validationRuleType: typeof(global::ValiCraft.Rules.NotEmpty),
        validationRuleGenericFormat: "")]
    public static global::ValiCraft.Abstractions.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsNotEmpty<TRequest, TPropertyType>(
        this global::ValiCraft.Abstractions.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder) where TRequest : class
        => throw new global::System.NotImplementedException("Never gets called");
}