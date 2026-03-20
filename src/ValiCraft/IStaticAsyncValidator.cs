using System.ComponentModel;

namespace ValiCraft;

/// <summary>
/// Defines a static async validator that validates instances of <typeparamref name="TRequest"/>.
/// Static validators are stateless and their validation methods can be called without instantiation.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IStaticAsyncValidator<TRequest> where TRequest : notnull
{
    /// <summary>
    /// Validates the specified request asynchronously.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    static abstract Task<ValidationErrors?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs the validation logic and returns the raw error list.
    /// This method is intended for internal use by generated code for efficient nested validation.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// A list of validation errors if validation fails; otherwise, <c>null</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    static abstract Task<List<ValidationError>?> RunValidationAsync(TRequest request, string? inheritedTargetPath, CancellationToken cancellationToken);
}
