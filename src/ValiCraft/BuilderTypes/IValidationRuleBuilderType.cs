namespace ValiCraft.BuilderTypes;

public interface IValidationRuleBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IWithMessageBuilderType<TRequest, TProperty> WithMessage(string message);
    IWithTargetNameBuilderType<TRequest, TProperty> WithTargetName(string targetName);
    IWithErrorCodeBuilderType<TRequest, TProperty> WithErrorCode(string errorCode);
}