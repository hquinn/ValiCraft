namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Builder type returned after setting metadata with <c>WithMetadata()</c>.
/// Allows further configuration with messages, error codes, target names, severity, and additional metadata.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
public interface IAsyncWithMetadataBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Adds additional metadata to the validation error.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithMetadataBuilderType<TRequest, TTarget> WithMetadata(string key, object value);

    /// <summary>
    /// Sets a custom error message for the validation rule.
    /// </summary>
    /// <param name="message">The error message, optionally containing placeholders.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);

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
    /// Sets a custom target name for display in error messages.
    /// </summary>
    /// <param name="targetName">The display name for the target property.</param>
    /// <returns>A builder for further configuration.</returns>
    IAsyncWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);
}
