using MonadCraft;

namespace ValiCraft;

/// <summary>
/// Defines an asynchronous validator that validates an entire request asynchronously.
/// This is completely separate from <see cref="IValidator{TRequest}"/> and supports
/// async validation rules that perform I/O operations.
/// </summary>
/// <typeparam name="TRequest">The type of request to validate.</typeparam>
public interface IAsyncValidator<TRequest>
{
    /// <summary>
    /// Validates the specified request asynchronously and returns a result containing either
    /// the valid request or a list of validation errors.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>
    /// A task that returns a <see cref="Result{TFailure, TSuccess}"/> containing either
    /// a list of <see cref="IValidationError"/> objects if validation fails,
    /// or the validated <typeparamref name="TRequest"/> if validation succeeds.
    /// </returns>
    Task<Result<IReadOnlyList<IValidationError>, TRequest>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the specified request asynchronously and returns a list of validation errors.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>
    /// A task that returns a read-only list of <see cref="IValidationError"/> objects.
    /// An empty list indicates successful validation.
    /// </returns>
    Task<IReadOnlyList<IValidationError>> ValidateToListAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
