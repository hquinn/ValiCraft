namespace ValiCraft.Attributes;

/// <summary>
/// Defines a placeholder that can be used in error messages for a validation rule.
/// The placeholder is replaced with the actual parameter value at runtime.
/// </summary>
/// <remarks>
/// <para>
/// This attribute allows custom validation rules to expose their constructor parameters
/// as placeholders in error messages. Multiple placeholders can be defined for a single rule.
/// </para>
/// <para>
/// Common use cases include showing comparison values, length constraints, or custom parameters
/// in validation error messages.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [GenerateRuleExtension("HasLengthBetween")]
/// [DefaultMessage("'{TargetName}' must have between {Min} and {Max} characters")]
/// [RulePlaceholder("Min", "minLength")]
/// [RulePlaceholder("Max", "maxLength")]
/// public class LengthBetweenRule : IValidationRule&lt;string?&gt;
/// {
///     private readonly int _minLength;
///     private readonly int _maxLength;
///
///     public LengthBetweenRule(int minLength, int maxLength)
///     {
///         _minLength = minLength;
///         _maxLength = maxLength;
///     }
///     // ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RulePlaceholderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RulePlaceholderAttribute"/> class.
    /// </summary>
    /// <param name="placeholderName">The name to use in the message template (e.g., "MinLength" for "{MinLength}").</param>
    /// <param name="parameterName">The name of the constructor parameter that provides the value.</param>
    public RulePlaceholderAttribute(string placeholderName, string parameterName)
    {
        PlaceholderName = placeholderName;
        ParameterName = parameterName;
    }

    /// <summary>
    /// Gets the placeholder name used in message templates (without braces).
    /// </summary>
    public string PlaceholderName { get; }

    /// <summary>
    /// Gets the name of the constructor parameter that provides the placeholder value.
    /// </summary>
    public string ParameterName { get; }
}