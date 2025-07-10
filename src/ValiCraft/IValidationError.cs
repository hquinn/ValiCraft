using MonadCraft.Errors;

namespace ValiCraft;

/// <summary>
/// Represents a validation failure for a specific property.
/// Inherits the base contract from MonadCraft.IError.
/// </summary>
public interface IValidationError : IError
{
    /// <summary>
    /// The name of the target that failed validation.
    /// </summary>
    string TargetName { get; }

    /// <summary>
    /// The value of the property that was attempted.
    /// </summary>
    object? AttemptedValue { get; }
}