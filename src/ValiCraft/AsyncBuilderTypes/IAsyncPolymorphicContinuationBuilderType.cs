namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Builder type for continuing to add type branches after a <c>WhenType&lt;T&gt;()</c> configuration.
/// Allows chaining additional type matches or defining fallback behavior.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The base type of the property being validated.</typeparam>
public interface IAsyncPolymorphicContinuationBuilderType<TRequest, TTarget>
    where TRequest : class
    where TTarget : class
{
    /// <summary>
    /// Defines a validation branch for when the target is of type <typeparamref name="TDerived"/>.
    /// </summary>
    /// <typeparam name="TDerived">The derived type to match.</typeparam>
    /// <returns>A builder for configuring the validation behavior for this type.</returns>
    IAsyncPolymorphicWhenBuilderType<TRequest, TTarget, TDerived> WhenType<TDerived>()
        where TDerived : class, TTarget;

    /// <summary>
    /// Defines fallback behavior when none of the specified types match.
    /// </summary>
    /// <returns>A builder for configuring the fallback behavior.</returns>
    IAsyncPolymorphicOtherwiseBuilderType<TRequest, TTarget> Otherwise();
}
