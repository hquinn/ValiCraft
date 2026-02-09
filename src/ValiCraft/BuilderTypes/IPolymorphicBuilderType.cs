namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned by <c>Polymorphic()</c> for validating a property based on its runtime type.
/// Provides access to type-specific validation branches.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The base type of the property being validated.</typeparam>
public interface IPolymorphicBuilderType<TRequest, TTarget>
    where TRequest : class
    where TTarget : class
{
    /// <summary>
    /// Defines a validation branch for when the target is of type <typeparamref name="TDerived"/>.
    /// </summary>
    /// <typeparam name="TDerived">The derived type to match.</typeparam>
    /// <returns>A builder for configuring the validation behavior for this type.</returns>
    IPolymorphicWhenBuilderType<TRequest, TTarget, TDerived> WhenType<TDerived>()
        where TDerived : class, TTarget;
}
