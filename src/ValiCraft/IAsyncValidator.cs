using System.ComponentModel;
using ErrorCraft;

namespace ValiCraft;

/// <summary>
/// Defines an async validator that validates instances of <typeparamref name="TRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IAsyncValidator<TRequest> where TRequest : notnull
{
    /// <summary>
    /// Validates the specified request asynchronously.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    Task<ValidationErrors?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the specified request with an inherited target path for nested validation.
    /// This method is intended for internal use by nested validators.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<ValidationErrors?> ValidateAsync(TRequest request, string? inheritedTargetPath, CancellationToken cancellationToken = default);
}
