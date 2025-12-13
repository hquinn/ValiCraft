namespace ValiCraft;

/// <summary>
/// Defines the severity level of a validation error.
/// </summary>
/// <remarks>
/// Use severity levels to categorize validation failures and implement
/// different handling strategies based on error importance.
/// </remarks>
public enum ErrorSeverity
{
    /// <summary>
    /// Informational message that does not necessarily indicate a problem.
    /// </summary>
    Info,

    /// <summary>
    /// Warning that indicates a potential issue but does not prevent validation from passing.
    /// </summary>
    Warning,

    /// <summary>
    /// Error that indicates a validation failure that should be addressed.
    /// </summary>
    Error
}