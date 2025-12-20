using ValiCraft.AsyncBuilderTypes;

namespace ValiCraft;

/// <summary>
/// Provides extension methods for async validation builder types, enabling nested object validation.
/// </summary>
public static class AsyncBuilderTypeExtensions
{
    /// <summary>
    /// Delegates validation of each item in a collection to another validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the collection item being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object property to validate.</typeparam>
    /// <typeparam name="TValidator">The type of the validator to use.</typeparam>
    /// <param name="builder">The validation builder for the collection item property.</param>
    /// <param name="validator">The validator instance to use for each item.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.EnsureEach(x => x.Orders, orderBuilder =>
    /// {
    ///     orderBuilder.Ensure(order => order.Customer)
    ///         .ValidateWith(new CustomerValidator());
    /// });
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget, TValidator>(
        this IAsyncEnsureEachBuilderType<TRequest, TTarget> builder,
        TValidator validator)
        where TRequest : class
        where TTarget : class
        where TValidator : IValidator<TTarget>
        => throw new NotImplementedException("Never gets called");

    /// <summary>
    /// Delegates validation of a nested object to another validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the parent object being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object to validate.</typeparam>
    /// <typeparam name="TValidator">The type of the validator to use.</typeparam>
    /// <param name="builder">The validation builder for the nested object property.</param>
    /// <param name="validator">The validator instance to use for the nested object.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Address)
    ///     .ValidateWith(new AddressValidator());
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget, TValidator>(
        this IAsyncEnsureBuilderType<TRequest, TTarget> builder, TValidator validator)
        where TRequest : class
        where TTarget : class
        where TValidator : IValidator<TTarget>
        => throw new NotImplementedException("Never gets called");
}