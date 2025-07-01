namespace ValiCraft.Abstractions.BuilderTypes;

public interface IWithErrorCodeBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TProperty> WithMessage(string message);
    IWithPropertyNameBuilderType<TRequest, TProperty> WithPropertyName(string propertyName);
}