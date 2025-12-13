namespace ValiCraft;

/// <summary>
/// Defines a validation rule that validates a target value.
/// Implement this interface to create custom validation rules.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
public interface IValidationRule<in TTargetValue>
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract bool IsValid(TTargetValue targetValue);
}

/// <summary>
/// Defines a validation rule that validates a target value with one parameter.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
public interface IValidationRule<in TTargetValue, in TParam1Value>
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract bool IsValid(TTargetValue targetValue, TParam1Value param1);
}

/// <summary>
/// Defines a validation rule that validates a target value with two parameters.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
/// <typeparam name="TParam2Value">The type of the second parameter.</typeparam>
public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value>
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="param2">The second parameter for validation.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract bool IsValid(TTargetValue targetValue, TParam1Value param1, TParam2Value param2);
}

/// <summary>
/// Defines a validation rule that validates a target value with three parameters.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
/// <typeparam name="TParam2Value">The type of the second parameter.</typeparam>
/// <typeparam name="TParam3Value">The type of the third parameter.</typeparam>
public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value, in TParam3Value>
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="param2">The second parameter for validation.</param>
    /// <param name="param3">The third parameter for validation.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract bool IsValid(
        TTargetValue targetValue,
        TParam1Value param1,
        TParam2Value param2,
        TParam3Value param3);
}

/// <summary>
/// Defines a validation rule that validates a target value with four parameters.
/// </summary>
/// <typeparam name="TTargetValue">The type of value to validate.</typeparam>
/// <typeparam name="TParam1Value">The type of the first parameter.</typeparam>
/// <typeparam name="TParam2Value">The type of the second parameter.</typeparam>
/// <typeparam name="TParam3Value">The type of the third parameter.</typeparam>
/// <typeparam name="TParam4Value">The type of the fourth parameter.</typeparam>
public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value, in TParam3Value, in TParam4Value>
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="targetValue">The value to validate.</param>
    /// <param name="param1">The first parameter for validation.</param>
    /// <param name="param2">The second parameter for validation.</param>
    /// <param name="param3">The third parameter for validation.</param>
    /// <param name="param4">The fourth parameter for validation.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    static abstract bool IsValid(
        TTargetValue targetValue,
        TParam1Value param1,
        TParam2Value param2,
        TParam3Value param3,
        TParam4Value param4);
}