namespace ValiCraft;

/// <summary>
/// Base class for creating static validators. Inherit from this class and apply the
/// <c>[GenerateValidator]</c> attribute to generate static validation logic at compile time.
/// Static validators are stateless and do not support constructor injection.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
/// <remarks>
/// The <c>DefineRules</c> method is analyzed by the source generator at compile time to
/// generate optimized static validation methods. The method itself is never called at runtime.
/// </remarks>
/// <example>
/// <code>
/// [GenerateValidator]
/// public partial class UserValidator : StaticValidator&lt;User&gt;
/// {
///     protected override void DefineRules(IValidationRuleBuilder&lt;User&gt; builder)
///     {
///         builder.Ensure(x => x.Name).IsNotNullOrWhiteSpace();
///     }
/// }
/// </code>
/// </example>
public abstract class StaticValidator<TRequest> where TRequest : class
{
    /// <summary>
    /// Defines the validation rules for <typeparamref name="TRequest"/>.
    /// Override this method to configure validation rules using the fluent builder API.
    /// This method is analyzed at compile time and is never called at runtime.
    /// </summary>
    /// <param name="builder">The validation rule builder used to define rules.</param>
    protected abstract void DefineRules(IValidationRuleBuilder<TRequest> builder);
}
