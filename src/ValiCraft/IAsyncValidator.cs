using System.ComponentModel;
using ErrorCraft;
using MonadCraft;

namespace ValiCraft;

/// <summary>
/// Defines an async validator that validates instances of <typeparamref name="TRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IAsyncValidator<TRequest> where TRequest : class
{
    /// <summary>
    /// Validates the specified request and returns a Result type.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing either the validation errors
    /// or the validated request on success.
    /// </returns>
    Task<Result<IValidationErrors, TRequest>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates the specified request and returns validation errors as a list.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of validation errors. Empty if validation succeeds.</returns>
    Task<IReadOnlyList<IValidationError>> ValidateToListAsync(TRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates the specified request with an inherited target path for nested validation.
    /// This method is intended for internal use by nested validators.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <returns>A list of validation errors. Empty if validation succeeds.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<IReadOnlyList<IValidationError>> ValidateToListAsync(TRequest request, string? inheritedTargetPath, CancellationToken cancellationToken = default);
}