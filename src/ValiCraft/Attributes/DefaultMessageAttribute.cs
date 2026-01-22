namespace ValiCraft.Attributes;

/// <summary>
/// Specifies the default error message to display when a validation rule fails.
/// The message supports placeholders that are replaced with actual values at runtime.
/// </summary>
/// <remarks>
/// <para>
/// Apply this attribute to custom validation rule classes to define their default error message.
/// The message can be overridden on a per-usage basis using <c>WithMessage()</c>.
/// </para>
/// <para>
/// Supported placeholders:
/// <list type="bullet">
///     <item><description><c>{TargetName}</c> - The name of the property being validated (humanized)</description></item>
///     <item><description><c>{TargetValue}</c> - The actual value being validated</description></item>
///     <item><description>Custom placeholders defined via <see cref="RulePlaceholderAttribute"/></description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [DefaultMessage("'{TargetName}' must have at least {MinLength} characters")]
/// [RulePlaceholder("MinLength", "minLength")]
/// public class MinLengthRule : IValidationRule&lt;string?&gt;
/// {
///     // ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DefaultMessageAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMessageAttribute"/> class.
    /// </summary>
    /// <param name="message">The default error message, optionally containing placeholders.</param>
    public DefaultMessageAttribute(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the default error message for the validation rule.
    /// </summary>
    public string Message { get; }
}