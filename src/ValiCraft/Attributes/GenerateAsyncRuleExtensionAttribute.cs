namespace ValiCraft.Attributes;

/// <summary>
/// Marks an async validation rule class for source generation, creating a fluent extension method
/// that can be used in async validator definitions.
/// </summary>
/// <remarks>
/// <para>
/// The source generator will create an extension method with the specified name that can be
/// chained in the async fluent validation API. The rule class must implement 
/// <c>IAsyncValidationRule&lt;TTarget&gt;</c>.
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
/// [GenerateAsyncRuleExtension("IsUniqueEmail")]
/// [DefaultMessage("'{TargetName}' must be unique")]
/// public class UniqueEmailRule : IAsyncValidationRule&lt;string&gt;
/// {
///     public static async Task&lt;bool&gt; IsValidAsync(string value, CancellationToken cancellationToken)
///     {
///         // Check database for uniqueness
///         return await emailRepository.IsUniqueAsync(value, cancellationToken);
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GenerateAsyncRuleExtensionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateAsyncRuleExtensionAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the extension method to generate (e.g., "IsUniqueEmailAsync").</param>
    public GenerateAsyncRuleExtensionAttribute(string name)
    {
        NameForRuleExtension = name;
    }

    /// <summary>
    /// Gets the name of the fluent extension method that will be generated.
    /// </summary>
    public string NameForRuleExtension { get; }
}
