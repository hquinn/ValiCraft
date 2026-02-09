namespace ValiCraft;

/// <summary>
/// Base class for creating asynchronous validators. Inherit from this class and apply the
/// <c>[GenerateValidator]</c> attribute to generate validation logic at compile time.
/// </summary>
/// <typeparam name="TRequest">The type of object to validate.</typeparam>
/// <example>
/// <code>
/// [GenerateValidator]
/// public partial class UserValidator : AsyncValidator&lt;User&gt;
/// {
///     protected override void DefineRules(IAsyncValidationRuleBuilder&lt;User&gt; builder)
///     {
///         builder.Ensure(x => x.Name).IsNotNullOrWhiteSpace();
///     }
/// }
/// </code>
/// </example>
public abstract class AsyncValidator<TRequest> where TRequest : class
{
    /// <summary>
    /// Defines the validation rules for <typeparamref name="TRequest"/>.
    /// Override this method to configure validation rules using the fluent builder API.
    /// </summary>
    /// <param name="builder">The validation rule builder used to define rules.</param>
    protected abstract void DefineRules(IAsyncValidationRuleBuilder<TRequest> builder);
}