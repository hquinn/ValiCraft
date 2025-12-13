using ValiCraft.BuilderTypes;

namespace ValiCraft;

/// <summary>
/// Provides a fluent builder interface for defining async validation rules.
/// This builder supports both synchronous rules and asynchronous rules
/// via <c>MustAsync</c> and typed async rule methods.
/// </summary>
/// <typeparam name="TRequest">The type of request being validated.</typeparam>
public interface IAsyncValidationRuleBuilder<TRequest>
    where TRequest : class
{
    /// <summary>
    /// Begins defining validation rules for a property or field of the request.
    /// </summary>
    /// <typeparam name="TTargetValue">The type of the property or field being validated.</typeparam>
    /// <param name="selector">An expression that selects the property or field to validate.</param>
    /// <returns>An async builder type for chaining validation rules.</returns>
    IAsyncBuilderType<TRequest, TTargetValue> Ensure<TTargetValue>(Func<TRequest, TTargetValue> selector);
}
