using System.ComponentModel;
using ErrorCraft;

namespace ValiCraft;

/// <summary>
/// Defines a static validator that validates instances of <typeparamref name="TRequest"/>.
/// Static validators are stateless and their validation methods can be called without instantiation.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IStaticValidator<TRequest> where TRequest : notnull
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    static abstract ValidationErrors? Validate(TRequest request);

    /// <summary>
    /// Validates the specified request with an inherited target path for nested validation.
    /// This method is intended for internal use by nested validators.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    static abstract ValidationErrors? Validate(TRequest request, string? inheritedTargetPath);
}
