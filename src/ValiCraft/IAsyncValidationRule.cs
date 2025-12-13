namespace ValiCraft;

/// <summary>
/// Defines an async validation rule that validates a target value asynchronously.
/// Implement this interface to create custom async validation rules that need to
/// perform I/O operations like database lookups or API calls.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
public interface IAsyncValidationRule<in TTargetValue>
{
    /// <summary>
    /// Determines whether the specified value is valid asynchronously.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A task that returns <c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract Task<bool> IsValidAsync(TTargetValue targetValue, CancellationToken cancellationToken);
}

/// <summary>
/// Defines an async validation rule that validates a target value with one parameter.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
public interface IAsyncValidationRule<in TTargetValue, in TParam1Value>
{
    /// <summary>
    /// Determines whether the specified value is valid asynchronously.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A task that returns <c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract Task<bool> IsValidAsync(TTargetValue targetValue, TParam1Value param1, CancellationToken cancellationToken);
}

/// <summary>
/// Defines an async validation rule that validates a target value with two parameters.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
/// <typeparam name="TParam2Value">The type of the second parameter.</typeparam>
public interface IAsyncValidationRule<in TTargetValue, in TParam1Value, in TParam2Value>
{
    /// <summary>
    /// Determines whether the specified value is valid asynchronously.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="param2">The second parameter for validation.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A task that returns <c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract Task<bool> IsValidAsync(TTargetValue targetValue, TParam1Value param1, TParam2Value param2, CancellationToken cancellationToken);
}

/// <summary>
/// Defines an async validation rule that validates a target value with three parameters.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
/// <typeparam name="TParam2Value">The type of the second parameter.</typeparam>
/// <typeparam name="TParam3Value">The type of the third parameter.</typeparam>
public interface IAsyncValidationRule<in TTargetValue, in TParam1Value, in TParam2Value, in TParam3Value>
{
    /// <summary>
    /// Determines whether the specified value is valid asynchronously.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="param2">The second parameter for validation.</param>
    /// <param name="param3">The third parameter for validation.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A task that returns <c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract Task<bool> IsValidAsync(
        TTargetValue targetValue,
        TParam1Value param1,
        TParam2Value param2,
        TParam3Value param3,
        CancellationToken cancellationToken);
}
