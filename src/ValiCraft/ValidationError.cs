namespace ValiCraft;

/// <summary>
/// A generic, type-safe representation of a single validation failure
/// for a property of type <typeparamref name="TTarget"/>.
/// </summary>
public readonly record struct ValidationError<TTarget> : IValidationError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public ErrorSeverity Severity { get; init; }
    public required string TargetName { get; init; }
    public required string TargetPath { get; init; }
    public required TTarget AttemptedValue { get; init; }

    // Explicit interface implementation to return the value as an object.
    object? IValidationError.AttemptedValue => this.AttemptedValue;
}