using ValiCraft.BuilderTypes;

namespace ValiCraft;

public static class BuilderTypeExtensions
{
    public static IValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget, TValidator>(
        this IEnsureEachBuilderType<TRequest, TTarget> builder,
        TValidator validator)
        where TRequest : class
        where TTarget : class
        where TValidator : IValidator<TTarget>
        => throw new NotImplementedException("Never gets called");
    
    public static IValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget, TValidator>(
        this IEnsureBuilderType<TRequest, TTarget> builder, TValidator validator)
        where TRequest : class
        where TTarget : class
        where TValidator : IValidator<TTarget>
        => throw new NotImplementedException("Never gets called");
}