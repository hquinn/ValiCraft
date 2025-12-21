namespace ValiCraft;

/// <summary>
/// A generic, type-safe representation of a single validation failure
/// for a property of type <typeparamref name="TTarget"/>.
/// </summary>
/// <typeparam name="TTarget">The type of the property that failed validation.</typeparam>
public readonly record struct ValidationError<TTarget> : IValidationError
{
    /// <inheritdoc />
    public required string Code { get; init; }

    /// <inheritdoc />
    public required string Message { get; init; }

    /// <inheritdoc />
    public ErrorSeverity Severity { get; init; }

    /// <inheritdoc />
    public required string TargetName { get; init; }

    /// <inheritdoc />
    public required string TargetPath { get; init; }

    /// <summary>
    /// Gets the value that was being validated when the error occurred.
    /// </summary>
    public required TTarget AttemptedValue { get; init; }

    // Explicit interface implementation to return the value as an object.
    object? IValidationError.AttemptedValue => this.AttemptedValue;
}