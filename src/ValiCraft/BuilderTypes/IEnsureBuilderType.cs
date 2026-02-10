namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned by <c>Ensure()</c> for validating a single property value.
/// Provides access to all validation rules and configuration options.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property being validated.</typeparam>
/// <remarks>
/// This is the primary entry point for adding validation rules to a property.
/// Chain validation rules and configure error messages, codes, and conditions.
/// </remarks>
public interface IEnsureBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    /// <summary>
    /// Delegates validation of the target property to a static validator.
    /// The validator must implement <see cref="IStaticValidator{T}"/> where T is the target type.
    /// </summary>
    /// <typeparam name="TValidator">The static validator type to use for validation.</typeparam>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <remarks>
    /// This method is only valid when <typeparamref name="TTarget"/> is a reference type.
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticValidator{T}"/> for the correct target type.
    /// </remarks>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Address)
    ///     .Validate&lt;AddressValidator&gt;();
    /// </code>
    /// </example>
    IStaticValidateBuilderType<TRequest, TTarget> Validate<TValidator>();
}