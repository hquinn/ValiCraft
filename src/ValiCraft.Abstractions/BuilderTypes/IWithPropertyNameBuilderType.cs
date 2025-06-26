namespace ValiCraft.Abstractions.BuilderTypes;

public interface IWithPropertyNameBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TProperty> WithMessage(string message);
    IWithErrorCodeBuilderType<TRequest, TProperty> WithErrorCode(string errorCode);
}