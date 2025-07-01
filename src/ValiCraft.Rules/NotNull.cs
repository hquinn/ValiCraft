using ValiCraft.Abstractions;
using ValiCraft.Abstractions.Attributes;

namespace ValiCraft.Rules;

//[GenerateRuleExtension("IsNotNull")]
public class NotNull<T> : IValidationRule<T?>
{
    public static bool IsValid(T? propertyValue)
    {
        return propertyValue is not null;
    }
}

    public static class NotNullExtensions
    {
        [global::ValiCraft.Abstractions.Attributes.MapToValidationRule(
            validationRuleType: typeof(global::ValiCraft.Rules.NotNull<>),
            validationRuleGenericFormat: "<{0}>")]
        public static global::ValiCraft.Abstractions.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsNotNull<TRequest, TPropertyType>(
            this global::ValiCraft.Abstractions.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder) where TRequest : class
            => throw new global::System.NotImplementedException("Never gets called");
    }


