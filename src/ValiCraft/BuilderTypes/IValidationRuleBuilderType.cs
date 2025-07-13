namespace ValiCraft.BuilderTypes;

public interface IValidationRuleBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);
    IWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);
    IWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);
}