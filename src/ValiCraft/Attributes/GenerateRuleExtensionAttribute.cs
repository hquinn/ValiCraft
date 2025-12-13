namespace ValiCraft.Attributes;

/// <summary>
/// Marks a validation rule class for source generation, creating a fluent extension method
/// that can be used in validator definitions.
/// </summary>
/// <remarks>
/// <para>
/// The source generator will create an extension method with the specified name that can be
/// chained in the fluent validation API. The rule class must implement <c>IValidationRule&lt;TTarget&gt;</c>.
/// </para>
/// <para>
/// Use in combination with:
/// <list type="bullet">
///     <item><description><see cref="DefaultMessageAttribute"/> - To specify the default error message</description></item>
///     <item><description><see cref="DefaultErrorCodeAttribute"/> - To specify the default error code</description></item>
///     <item><description><see cref="RulePlaceholderAttribute"/> - To define message placeholders</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [GenerateRuleExtension("IsValidPostalCode")]
/// [DefaultMessage("'{TargetName}' must be a valid postal code")]
/// public class PostalCodeRule : IValidationRule&lt;string?&gt;
/// {
///     public static bool IsValid(string? value)
///     {
///         if (string.IsNullOrEmpty(value)) return false;
///         return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GenerateRuleExtensionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateRuleExtensionAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the extension method to generate (e.g., "IsValidPostalCode").</param>
    public GenerateRuleExtensionAttribute(string name)
    {
        NameForRuleExtension = name;
    }

    /// <summary>
    /// Gets the name of the fluent extension method that will be generated.
    /// </summary>
    public string NameForRuleExtension { get; }
}