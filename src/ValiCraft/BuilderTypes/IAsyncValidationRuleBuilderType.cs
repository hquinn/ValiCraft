namespace ValiCraft.BuilderTypes;

/// <summary>
/// Async builder type returned after applying a validation rule, providing configuration options
/// and the ability to chain additional rules including async rules.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
/// <remarks>
/// This is the most commonly used async builder type in the fluent API. It allows you to:
/// <list type="bullet">
///     <item><description>Chain additional validation rules (sync or async)</description></item>
///     <item><description>Set custom error messages with <see cref="WithMessage"/></description></item>
///     <item><description>Set custom target names with <see cref="WithTargetName"/></description></item>
///     <item><description>Set error codes with <see cref="WithErrorCode"/></description></item>
///     <item><description>Apply conditional validation with <see cref="If"/></description></item>
/// </list>
/// </remarks>
public interface IAsyncValidationRuleBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Sets a custom error message for the validation rule.
    /// </summary>
    /// <param name="message">The error message, optionally containing placeholders like {TargetName}, {TargetValue}.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);

    /// <summary>
    /// Sets a custom display name for the validated property in error messages.
    /// </summary>
    /// <param name="targetName">The display name to use instead of the property name.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);

    /// <summary>
    /// Sets a custom error code for programmatic error handling.
    /// </summary>
    /// <param name="errorCode">The error code identifier.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);

    /// <summary>
    /// Sets the severity level for the validation error.
    /// </summary>
    /// <param name="severity">The severity level (Info, Warning, or Error).</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithSeverityBuilderType<TRequest, TTarget> WithSeverity(ErrorSeverity severity);

    /// <summary>
    /// Applies a condition that must be met for the validation rule to execute.
    /// </summary>
    /// <param name="condition">A predicate that returns <c>true</c> if the rule should be applied.</param>
    /// <returns>A builder for configuring the conditional rule.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.PhoneNumber)
    ///     .IsNotNullOrWhiteSpace()
    ///     .If(x => x.ContactMethod == "Phone");
    /// </code>
    /// </example>
    IAsyncIfBuilderType<TRequest, TTarget> If(Func<TRequest, bool> condition);
}
