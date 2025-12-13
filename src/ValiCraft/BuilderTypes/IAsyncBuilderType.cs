namespace ValiCraft.BuilderTypes;

/// <summary>
/// Base interface for all async validation builder types, providing core validation capabilities
/// including async predicate validation via <see cref="MustAsync"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property or value being validated.</typeparam>
/// <remarks>
/// This interface defines the foundation for the async fluent validation API. It supports both
/// synchronous validation rules (via <see cref="Must"/>) and asynchronous rules (via <see cref="MustAsync"/>).
/// </remarks>
public interface IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Adds a custom validation rule using a synchronous predicate function.
    /// </summary>
    /// <param name="predicate">A function that returns <c>true</c> if the value is valid, <c>false</c> otherwise.</param>
    /// <returns>A builder for configuring additional options like error messages and codes.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Password)
    ///     .Must(password => password.Any(char.IsDigit))
    ///     .WithMessage("Password must contain at least one digit");
    /// </code>
    /// </example>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);

    /// <summary>
    /// Adds a custom validation rule using an asynchronous predicate function.
    /// Use this for validation logic that requires I/O operations.
    /// </summary>
    /// <param name="predicate">An async function that returns <c>true</c> if the value is valid, <c>false</c> otherwise.</param>
    /// <returns>A builder for configuring additional options like error messages and codes.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Email)
    ///     .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
    ///     .WithMessage("Email must be unique");
    /// </code>
    /// </example>
    IAsyncValidationRuleBuilderType<TRequest, TTarget> MustAsync(Func<TTarget, CancellationToken, Task<bool>> predicate);

    /// <summary>
    /// Indicates that subsequent validation rules should only be applied if the target value is not null.
    /// This is useful for optional nullable properties where you want to validate the value only when present.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.OptionalEmail)
    ///     .WhenNotNull()
    ///     .IsEmailAddress()
    ///     .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct));
    /// </code>
    /// </example>
    IAsyncWhenNotNullBuilderType<TRequest, TTarget> WhenNotNull();
}
