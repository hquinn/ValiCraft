using System.ComponentModel;
using ErrorCraft;

namespace ValiCraft;

/// <summary>
/// Defines a validator that validates instances of <typeparamref name="TRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
public interface IValidator<TRequest> where TRequest : notnull
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <returns>
    /// A <see cref="ValidationErrors"/> if validation fails; otherwise, <c>null</c>.
    /// </returns>
    ValidationErrors? Validate(TRequest request);

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
    ValidationErrors? Validate(TRequest request, string? inheritedTargetPath);

    /// <summary>
    /// Runs the validation logic and returns the raw error list.
    /// This method is intended for internal use by generated code for efficient nested validation.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <returns>
    /// A list of validation errors if validation fails; otherwise, <c>null</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    List<IValidationError>? RunValidation(TRequest request, string? inheritedTargetPath);
}
