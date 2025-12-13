namespace ValiCraft.Attributes;

/// <summary>
/// Marks a partial async validator class for source generation.
/// The source generator will analyze the <c>DefineRules</c> method and generate
/// optimized asynchronous validation code at compile time.
/// </summary>
/// <remarks>
/// <para>
/// Classes decorated with this attribute must:
/// <list type="bullet">
///     <item><description>Be declared as <c>partial</c></description></item>
///     <item><description>Inherit from <see cref="AsyncValidator{TRequest}"/></description></item>
///     <item><description>Override the <c>DefineRules</c> method</description></item>
/// </list>
/// </para>
/// <para>
/// The generated code implements the actual validation logic with support for
/// asynchronous operations like database queries, API calls, or file I/O.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [GenerateAsyncValidator]
/// public partial class UserValidator : AsyncValidator&lt;User&gt;
/// {
///     protected override void DefineRules(IAsyncValidationRuleBuilder&lt;User&gt; builder)
///     {
///         builder.Ensure(x => x.Email)
///             .IsNotNullOrWhiteSpace()
///             .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct));
///     }
///     
///     private async Task&lt;bool&gt; IsEmailUniqueAsync(string email, CancellationToken ct)
///     {
///         // Check database for uniqueness
///         return true;
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GenerateAsyncValidatorAttribute : Attribute
{
}
