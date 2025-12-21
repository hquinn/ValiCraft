namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned after setting a target name with <c>WithTargetName()</c>.
/// Allows further configuration of messages and error codes.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
public interface IWithTargetNameBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Sets a custom error message for the validation rule.
    /// </summary>
    /// <param name="message">The error message, optionally containing placeholders.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);

    /// <summary>
    /// Sets a custom error code for programmatic error handling.
    /// </summary>
    /// <param name="errorCode">The error code identifier.</param>
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