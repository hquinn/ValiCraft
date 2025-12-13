namespace ValiCraft.BuilderTypes;

/// <summary>
/// An async builder type returned by WhenNotNull() that allows chaining validation rules
/// that will only execute if the target value is not null.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
public interface IAsyncWhenNotNullBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Apply a custom synchronous validation predicate that only runs if the value is not null.
    /// </summary>
    /// <param name="predicate">A function that returns <c>true</c> if the value is valid.</param>
    /// <returns>A builder for further configuration.</returns>
    new IAsyncValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);

    /// <summary>
    /// Apply a custom asynchronous validation predicate that only runs if the value is not null.
    /// </summary>
    /// <param name="predicate">An async function that returns <c>true</c> if the value is valid.</param>
    /// <returns>A builder for further configuration.</returns>
    new IAsyncValidationRuleBuilderType<TRequest, TTarget> MustAsync(Func<TTarget, CancellationToken, Task<bool>> predicate);
}
