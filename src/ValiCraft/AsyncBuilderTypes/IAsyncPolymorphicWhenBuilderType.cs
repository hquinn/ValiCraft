namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Builder type returned by <c>WhenType&lt;T&gt;()</c> for configuring validation behavior 
/// when the target matches a specific derived type.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The base type of the property being validated.</typeparam>
/// <typeparam name="TDerived">The derived type being matched.</typeparam>
public interface IAsyncPolymorphicWhenBuilderType<TRequest, TTarget, TDerived>
    where TRequest : class
    where TTarget : class
    where TDerived : class, TTarget
{
    /// <summary>
    /// Validates the target using the specified sync validator when it matches this type.
    /// </summary>
    /// <param name="validator">The validator to use for this type.</param>
    /// <returns>A builder for chaining additional type branches.</returns>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> ValidateWith(IValidator<TDerived> validator);

    /// <summary>
    /// Validates the target using the specified async validator when it matches this type.
    /// </summary>
    /// <param name="validator">The async validator to use for this type.</param>
    /// <returns>A builder for chaining additional type branches.</returns>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> ValidateWith(IAsyncValidator<TDerived> validator);

    /// <summary>
    /// Validates the target using the specified static sync validator when it matches this type.
    /// The validator must implement <see cref="IStaticValidator{T}"/> where T is the derived type.
    /// </summary>
    /// <typeparam name="TValidator">The static validator type to use for this type.</typeparam>
    /// <returns>A builder for chaining additional type branches.</returns>
    /// <remarks>
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticValidator{T}"/> for the correct derived type.
    /// </remarks>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> Validate<TValidator>();

    /// <summary>
    /// Validates the target using the specified static async validator when it matches this type.
    /// The validator must implement <see cref="IStaticAsyncValidator{T}"/> where T is the derived type.
    /// </summary>
    /// <typeparam name="TValidator">The static async validator type to use for this type.</typeparam>
    /// <returns>A builder for chaining additional type branches.</returns>
    /// <remarks>
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticAsyncValidator{T}"/> for the correct derived type.
    /// </remarks>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> ValidateAsync<TValidator>();

    /// <summary>
    /// Allows validation to pass when the target matches this type without further validation.
    /// </summary>
    /// <returns>A builder for chaining additional type branches.</returns>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> Allow();

    /// <summary>
    /// Fails validation when the target matches this type.
    /// </summary>
    /// <returns>A builder for chaining additional type branches.</returns>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> Fail();

    /// <summary>
    /// Fails validation with a custom message when the target matches this type.
    /// </summary>
    /// <param name="message">The error message. Supports placeholders like {TargetName}.</param>
    /// <returns>A builder for chaining additional type branches.</returns>
    IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget> Fail(string message);
}
