namespace ValiCraft.BuilderTypes;

public interface IWithMessageBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithTargetNameBuilderType<TRequest, TProperty> WithTargetName(string targetName);
    IWithErrorCodeBuilderType<TRequest, TProperty> WithErrorCode(string errorCode);
}