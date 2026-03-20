namespace ValiCraft;

/// <summary>
/// Represents the result of a failed validation, containing one or more validation errors.
/// </summary>
public readonly struct ValidationErrors
{
    /// <summary>
    /// A unique identifier for the validation errors, typically derived from the request type.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// A summary message describing the validation failure.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// The overall severity level of the validation failure.
    /// </summary>
    public ErrorSeverity Severity { get; init; }

    /// <summary>
    /// The individual validation errors.
    /// </summary>
    public required IReadOnlyList<ValidationError> Errors { get; init; }

    /// <summary>
    /// Optional metadata associated with the validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
}
