namespace ValiCraft.BuilderTypes;

public interface IWithErrorCodeBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TProperty> WithMessage(string message);
    IWithTargetNameBuilderType<TRequest, TProperty> WithTargetName(string targetName);
}