using ValiCraft.BuilderTypes;

namespace ValiCraft;

public static class BuilderTypeExtensions
{
    public static IValidateWithBuilderType<TRequest, TProperty> ValidateWith<TRequest, TProperty, TValidator>(
        this IEnsureEachBuilderType<TRequest, TProperty> builder,
        TValidator validator)
        where TRequest : class
        where TProperty : class
        where TValidator : IValidator<TProperty>
        => throw new NotImplementedException("Never gets called");
    
    public static IValidateWithBuilderType<TRequest, TProperty> ValidateWith<TRequest, TProperty, TValidator>(
        this IEnsureBuilderType<TRequest, TProperty> builder, TValidator validator)
        where TRequest : class
        where TProperty : class
        where TValidator : IValidator<TProperty>
        => throw new NotImplementedException("Never gets called");
}