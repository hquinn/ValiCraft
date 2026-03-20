namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned after setting a severity with <c>WithSeverity()</c>.
/// Allows further configuration of messages, target names, and error codes.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
public interface IWithSeverityBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : notnull
{
    /// <summary>
    /// Sets a custom error message for the validation rule.
    /// </summary>
    /// <param name="message">The error message, optionally containing placeholders.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithMessageBuilderType<TRequest, TTarget> WithMessage(string message);

    /// <summary>
    /// Sets a custom display name for the validated property in error messages.
    /// </summary>
    /// <param name="targetName">The display name to use instead of the property name.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithTargetNameBuilderType<TRequest, TTarget> WithTargetName(string targetName);

    /// <summary>
    /// Sets a custom error code for programmatic error handling.
    /// </summary>
    /// <param name="errorCode">The error code identifier.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithErrorCodeBuilderType<TRequest, TTarget> WithErrorCode(string errorCode);

    /// <summary>
    /// Adds metadata to the validation error.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A builder for further configuration.</returns>
    IWithMetadataBuilderType<TRequest, TTarget> WithMetadata(string key, object value);

    /// <summary>
    /// Delegates validation of the target property to a static validator.
    /// The validator must implement <see cref="IStaticValidator{T}"/> where T is the target type.
    /// </summary>
    /// <typeparam name="TValidator">The static validator type to use for validation.</typeparam>
    /// <returns>A builder for chaining additional validation rules.</returns>
    IStaticValidateBuilderType<TRequest, TTarget> Validate<TValidator>();
}
