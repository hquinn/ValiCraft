namespace ValiCraft.BuilderTypes;

public interface IWithMessageBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithPropertyNameBuilderType<TRequest, TProperty> WithPropertyName(string propertyName);
    IWithErrorCodeBuilderType<TRequest, TProperty> WithErrorCode(string errorCode);
}