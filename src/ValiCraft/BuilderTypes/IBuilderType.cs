namespace ValiCraft.BuilderTypes;

/// <summary>
/// Base interface for all validation builder types, providing core validation capabilities.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property or value being validated.</typeparam>
/// <remarks>
/// This interface defines the foundation for the fluent validation API, including custom predicate
/// validation via <see cref="Must"/> and optional validation via <see cref="WhenNotNull"/>.
/// </remarks>
public interface IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Adds a custom validation rule using a predicate function.
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
    IValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);
    
    /// <summary>
    /// Indicates that subsequent validation rules should only be applied if the target value is not null.
    /// This is useful for optional nullable properties where you want to validate the value only when present.
    /// </summary>
    /// <example>
    /// builder.Ensure(x => x.OptionalEmail)
    ///     .WhenNotNull()
    ///     .IsEmailAddress()
    ///     .HasMaxLength(255);
    /// </example>
    IWhenNotNullBuilderType<TRequest, TTarget> WhenNotNull();
}