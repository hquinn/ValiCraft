namespace ValiCraft.Attributes;

/// <summary>
/// Specifies the default error code to use when a validation rule fails.
/// This code can be used for programmatic error handling and localization.
/// </summary>
/// <remarks>
/// Apply this attribute to custom validation rule classes to define their default error code.
/// The error code can be overridden on a per-usage basis using <c>WithErrorCode()</c>.
/// </remarks>
/// <example>
/// <code>
/// [GenerateRuleExtension("IsValidEmail")]
/// [DefaultErrorCode("EMAIL_INVALID")]
/// public class EmailRule : IValidationRule&lt;string?&gt;
/// {
///     // ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DefaultErrorCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultErrorCodeAttribute"/> class.
    /// </summary>
    /// <param name="errorCode">The default error code for the validation rule.</param>
    public DefaultErrorCodeAttribute(string errorCode)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Gets the default error code for the validation rule.
    /// </summary>
    public string ErrorCode { get; }
}