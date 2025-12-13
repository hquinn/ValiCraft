namespace ValiCraft;

/// <summary>
/// Base class for creating asynchronous validators. Derive from this class
/// and override <see cref="DefineRules"/> to define your validation rules.
/// The source generator will generate the <see cref="IAsyncValidator{TRequest}"/>
/// implementation based on your rule definitions.
/// </summary>
/// <typeparam name="TRequest">The type of request to validate.</typeparam>
/// <remarks>
/// <para>
/// Use <see cref="AsyncValidator{TRequest}"/> when your validation logic requires
/// asynchronous operations such as database lookups, API calls, or file I/O.
/// </para>
/// <para>
/// For purely synchronous validation, use <see cref="Validator{TRequest}"/> instead.
/// </para>
/// <example>
/// <code>
/// [GenerateAsyncValidator]
/// public partial class UserValidator : AsyncValidator&lt;User&gt;
/// {
///     protected override void DefineRules(IAsyncValidationRuleBuilder&lt;User&gt; builder)
///     {
///         builder.Ensure(x => x.Email)
///             .NotEmpty()
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
/// </remarks>
public abstract class AsyncValidator<TRequest>
    where TRequest : class
{
    /// <summary>
    /// Defines the validation rules for the request type.
    /// Override this method to configure your validation rules using the provided builder.
    /// </summary>
    /// <param name="builder">The builder used to define validation rules.</param>
    protected abstract void DefineRules(IAsyncValidationRuleBuilder<TRequest> builder);
}
