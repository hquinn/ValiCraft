using ValiCraft.Abstractions;
using ValiCraft.Abstractions.Attributes;

namespace ValiCraft.Rules;

// [GenerateRuleExtension("IsGreaterThan")]
public class GreaterThan<TPropertyType> : IValidationRule<TPropertyType, TPropertyType>
    where TPropertyType : IComparable
{
    public static bool IsValid(TPropertyType property, TPropertyType parameter)
    {
        return property.CompareTo(parameter) > 0;
    }
}


public static class GreaterThanExtensions
{
    [global::ValiCraft.Abstractions.Attributes.MapToValidationRule(
        validationRuleType: typeof(global::ValiCraft.Rules.GreaterThan<>),
        validationRuleGenericFormat: "<{0}>")]
    public static global::ValiCraft.Abstractions.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsGreaterThan<TRequest, TPropertyType>(
        this global::ValiCraft.Abstractions.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder, TPropertyType parameter) where TRequest : class
        => throw new global::System.NotImplementedException("Never gets called");
}