namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned by <c>WhenType&lt;T&gt;()</c> for configuring validation behavior 
/// when the target matches a specific derived type.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The base type of the property being validated.</typeparam>
/// <typeparam name="TDerived">The derived type being matched.</typeparam>
public interface IPolymorphicWhenBuilderType<TRequest, TTarget, TDerived>
    where TRequest : class
    where TTarget : class
    where TDerived : class, TTarget
{
    /// <summary>
    /// Validates the target using the specified validator when it matches this type.
    /// </summary>
    /// <param name="validator">The validator to use for this type.</param>
    /// <returns>A builder for chaining additional type branches.</returns>
    IPolymorphicContinuationBuilderType<TRequest, TTarget> ValidateWith(IValidator<TDerived> validator);

    /// <summary>
    /// Validates the target using the specified static validator when it matches this type.
    /// The validator must implement <see cref="IStaticValidator{T}"/> where T is the derived type.
    /// </summary>
    /// <typeparam name="TValidator">The static validator type to use for this type.</typeparam>
    /// <returns>A builder for chaining additional type branches.</returns>
    /// <remarks>
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticValidator{T}"/> for the correct derived type.
    /// </remarks>
    IPolymorphicContinuationBuilderType<TRequest, TTarget> Validate<TValidator>();

    /// <summary>
    /// Allows validation to pass when the target matches this type without further validation.
    /// </summary>
    /// <returns>A builder for chaining additional type branches.</returns>
    IPolymorphicContinuationBuilderType<TRequest, TTarget> Allow();

    /// <summary>
    /// Fails validation when the target matches this type.
    /// </summary>
    /// <returns>A builder for chaining additional type branches.</returns>
    IPolymorphicContinuationBuilderType<TRequest, TTarget> Fail();

    /// <summary>
    /// Fails validation with a custom message when the target matches this type.
    /// </summary>
    /// <param name="message">The error message. Supports placeholders like {TargetName}.</param>
    /// <returns>A builder for chaining additional type branches.</returns>
    IPolymorphicContinuationBuilderType<TRequest, TTarget> Fail(string message);
}
