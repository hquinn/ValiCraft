namespace ValiCraft.BuilderTypes;

public interface IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);
}