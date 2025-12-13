namespace ValiCraft;

/// <summary>
/// Specifies the behavior when a validation rule fails.
/// </summary>
/// <remarks>
/// This mode is specified when calling <c>Ensure()</c> to control whether subsequent
/// validation rules should be executed after a failure.
/// </remarks>
public enum OnFailureMode
{
    /// <summary>
    /// Continue validating remaining rules even if this rule fails.
    /// All validation errors will be collected. This is the default behavior.
    /// </summary>
    Continue,

    /// <summary>
    /// Stop validating the current property chain if this rule fails.
    /// Useful for preventing cascading errors when a fundamental check fails.
    /// </summary>
    /// <example>
    /// <code>
    /// // If Age is negative, don't check the upper bound
    /// builder.Ensure(x => x.Age, OnFailureMode.Halt)
    ///     .IsGreaterOrEqualThan(0)
    ///     .IsLessThan(150);
    /// </code>
    /// </example>
    Halt
}