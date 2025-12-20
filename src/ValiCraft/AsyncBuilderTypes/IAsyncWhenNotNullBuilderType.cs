namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// A builder type returned by WhenNotNull() that allows chaining validation rules
/// that will only execute if the target value is not null.
/// </summary>
public interface IAsyncWhenNotNullBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Apply a custom validation predicate that only runs if the value is not null.
    /// </summary>
    new IAsyncValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);
}
