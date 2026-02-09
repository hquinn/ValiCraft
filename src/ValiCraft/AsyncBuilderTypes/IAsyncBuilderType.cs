namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Base interface for all validation builder types, providing core validation capabilities.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property or value being validated.</typeparam>
/// <remarks>
/// This interface defines the foundation for the fluent validation API, including custom predicate
/// validation via Is.
/// </remarks>
public interface IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <returns>A builder for chaining additional validation rules on the target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is(
        Func<TTarget, bool> rule);

    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter">The parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam">The type of the parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam>(
        Func<TTarget, TParam, bool> rule,
        TParam parameter);

    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2>(
        Func<TTarget, TParam1, TParam2, bool> rule,
        TParam1 parameter1,
        TParam2 parameter2);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3>(
        Func<TTarget, TParam1, TParam2, TParam3, bool> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, bool> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter5">The fifth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam5">The type of the fifth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4, TParam5>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, TParam5, bool> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4,
        TParam5 parameter5);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter5">The fifth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter6">The sixth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam5">The type of the fifth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam6">The type of the sixth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, bool> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4,
        TParam5 parameter5,
        TParam6 parameter6);
    
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <returns>A builder for chaining additional validation rules on the target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is(
        Func<TTarget, CancellationToken, Task<bool>> rule);

    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter">The parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam">The type of the parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam>(
        Func<TTarget, TParam, CancellationToken, Task<bool>> rule,
        TParam parameter);

    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2>(
        Func<TTarget, TParam1, TParam2, CancellationToken, Task<bool>> rule,
        TParam1 parameter1,
        TParam2 parameter2);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3>(
        Func<TTarget, TParam1, TParam2, TParam3, CancellationToken, Task<bool>> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, CancellationToken, Task<bool>> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter5">The fifth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam5">The type of the fifth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4, TParam5>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, TParam5, CancellationToken, Task<bool>> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4,
        TParam5 parameter5);
    
    /// <summary>
    /// Adds a validation rule for the target based on a specified condition.
    /// </summary>
    /// <param name="rule">A function that defines the condition to validate the target.
    /// Returns true if the target satisfies the condition, otherwise false.</param>
    /// <param name="parameter1">The first parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter2">The second parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter3">The third parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter4">The fourth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter5">The fifth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <param name="parameter6">The sixth parameter that will be passed to <paramref name="rule"/> function.</param>
    /// <typeparam name="TParam1">The type of the first parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam5">The type of the fifth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <typeparam name="TParam6">The type of the sixth parameter used in the <paramref name="rule"/> function.</typeparam>
    /// <returns>A validation rule builder for chaining further validation rules on the current target.</returns>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Is<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
        Func<TTarget, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, CancellationToken, Task<bool>> rule,
        TParam1 parameter1,
        TParam2 parameter2,
        TParam3 parameter3,
        TParam4 parameter4,
        TParam5 parameter5,
        TParam6 parameter6);
}