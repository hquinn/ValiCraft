namespace ValiCraft.Attributes;

/// <summary>
/// Marks a partial asynchronous validator class for source generation.
/// The source generator will analyze the <c>DefineRules</c> method and generate
/// optimized validation code at compile time.
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
/// The generated code implements the actual validation logic with optimal performance
/// and minimal allocations.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [GenerateValidator]
/// public partial class UserValidator : AsyncValidator&lt;User&gt;
/// {
///     protected override void DefineRules(IAsyncValidationRuleBuilder&lt;User&gt; builder)
///     {
///         builder.Ensure(x => x.Username)
///             .IsNotNullOrWhiteSpace()
///             .HasMinLength(3);
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AsyncGenerateValidatorAttribute : Attribute
{
}