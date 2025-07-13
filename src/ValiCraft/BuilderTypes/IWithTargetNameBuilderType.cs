namespace ValiCraft.BuilderTypes;

public interface IWithTargetNameBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);
    IWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);
}