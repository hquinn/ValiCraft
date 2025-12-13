namespace ValiCraft;

/// <summary>
/// Represents a validation failure for a specific property.
/// </summary>
public interface IValidationError
{
    /// <summary>
    /// A unique identifier for the specific validation error, typically used
    /// to categorize or look up error details.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Describes the details of the validation failure, providing human-readable
    /// information about what went wrong or the reason for the validation error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Indicates the severity level of the validation error,
    /// specifying whether the issue is critical, a warning, or informational.
    /// </summary>
    public ErrorSeverity Severity { get; }

    /// <summary>
    /// Specifies the full path of the target property or field where the validation error occurred,
    /// allowing precise identification of the location in complex object graphs.
    /// </summary>
    public string TargetPath { get; }
    
    /// <summary>
    /// The name of the target that failed validation.
    /// </summary>
    string TargetName { get; }

    /// <summary>
    /// The value of the property that was attempted.
    /// </summary>
    object? AttemptedValue { get; }
}