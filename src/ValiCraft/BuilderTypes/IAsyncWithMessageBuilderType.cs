namespace ValiCraft.BuilderTypes;

/// <summary>
/// Async builder type returned after setting a message with <c>WithMessage()</c>.
/// Allows further configuration of target names and error codes.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
public interface IAsyncWithMessageBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
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
}
