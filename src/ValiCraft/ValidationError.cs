namespace ValiCraft;

/// <summary>
/// Represents a single validation failure for a property.
/// </summary>
public readonly struct ValidationError
{
    /// <summary>
    /// A unique identifier for the specific error, typically used to categorize or look up error details.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Describes the details of the failure, providing human-readable
    /// information about what went wrong or the reason for the error.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Indicates the severity level of the error.
    /// </summary>
    public ErrorSeverity Severity { get; init; }

    /// <summary>
    /// The name of the target that failed validation.
    /// </summary>
    public required string TargetName { get; init; }

    /// <summary>
    /// The full path of the target property or field where the validation error occurred.
    /// </summary>
    public required string TargetPath { get; init; }

    /// <summary>
    /// The value of the property that was attempted.
    /// </summary>
    public object? AttemptedValue { get; init; }

    /// <summary>
    /// Optional metadata associated with the error.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
}
