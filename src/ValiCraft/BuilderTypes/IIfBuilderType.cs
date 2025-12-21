namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned after applying an <c>If()</c> condition to a validation rule.
/// Allows further configuration of the conditional validation.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
/// <remarks>
/// When a condition is applied via <c>If()</c>, the validation rule only executes
/// when the condition returns <c>true</c>. This builder allows additional configuration
/// such as custom messages and error codes for the conditional rule.
/// </remarks>
public interface IIfBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Sets a custom error message for the validation rule.
    /// </summary>
    /// <param name="message">The error message, optionally containing placeholders.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);

    /// <summary>
    /// Sets a custom target name for use in error messages.
    /// </summary>
    /// <param name="targetName">The display name for the validated property.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);

    /// <summary>
    /// Sets a custom error code for the validation rule.
    /// </summary>
    /// <param name="errorCode">The error code for programmatic error handling.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);

    /// <summary>
    /// Sets the severity level for the validation error.
    /// </summary>
    /// <param name="severity">The severity level (Info, Warning, or Error).</param>
    /// <returns>A builder for further configuration.</returns>
    IWithSeverityBuilderType<TRequest, TTarget> WithSeverity(ErrorSeverity severity);

    /// <summary>
    /// Adds metadata to the validation error.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithMetadataBuilderType<TRequest, TTarget> WithMetadata(string key, object value);
}