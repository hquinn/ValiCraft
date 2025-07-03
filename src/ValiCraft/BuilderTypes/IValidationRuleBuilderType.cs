namespace ValiCraft.BuilderTypes;

public interface IValidationRuleBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TProperty> WithMessage(string message);
    IWithPropertyNameBuilderType<TRequest, TProperty> WithPropertyName(string propertyName);
    IWithErrorCodeBuilderType<TRequest, TProperty> WithErrorCode(string errorCode);
}