using System.ComponentModel;

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
    /// Runs the validation logic and returns the raw error list.
    /// This method is intended for internal use by generated code for efficient nested validation.
    /// </summary>
    /// <param name="request">The object to validate.</param>
    /// <param name="inheritedTargetPath">The path prefix from parent validators.</param>
    /// <returns>
    /// A list of validation errors if validation fails; otherwise, <c>null</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    static abstract List<ValidationError>? RunValidation(TRequest request, string? inheritedTargetPath);
}
