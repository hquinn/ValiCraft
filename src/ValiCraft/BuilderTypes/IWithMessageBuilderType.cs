namespace ValiCraft.BuilderTypes;

public interface IWithMessageBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);
    IWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);
}