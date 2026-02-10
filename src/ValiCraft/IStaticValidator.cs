using System.ComponentModel;
using ErrorCraft;
using MonadCraft;

namespace ValiCraft;

/// <summary>
/// Defines a static validator that validates instances of <typeparamref name="TRequest"/>.
/// Static validators are stateless and their validation methods can be called without instantiation.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IStaticValidator<TRequest> where TRequest : class
{
    /// <summary>
    /// Validates the specified request and returns a Result type.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing either the validation errors
    /// or the validated request on success.
    /// </returns>
    static abstract Result<IValidationErrors, TRequest> Validate(TRequest request);
    
    /// <summary>
    /// Validates the specified request and returns validation errors as a list.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <returns>A list of validation errors. Empty if validation succeeds.</returns>
    static abstract IReadOnlyList<IValidationError> ValidateToList(TRequest request);
    
    /// <summary>
    /// Validates the specified request with an inherited target path for nested validation.
    /// This method is intended for internal use by nested validators.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <returns>A list of validation errors. Empty if validation succeeds.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    static abstract IReadOnlyList<IValidationError> ValidateToList(TRequest request, string? inheritedTargetPath);
}
