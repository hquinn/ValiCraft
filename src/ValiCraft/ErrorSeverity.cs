namespace ValiCraft;

/// <summary>
/// Defines the severity level of a validation error.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Informational message that does not necessarily indicate a problem.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning that indicates a potential issue.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error that indicates a failure that should be addressed.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Indicates a critical level of severity, representing a severe error
    /// condition that typically requires immediate attention or intervention.
    /// </summary>
    Critical = 3
}
