using ValiCraft.AsyncBuilderTypes;

namespace ValiCraft;

/// <summary>
/// Provides extension methods for async validation builder types, enabling nested object validation.
/// </summary>
public static class AsyncBuilderTypeExtensions
{
    /// <summary>
    /// Delegates validation of each item in a collection to a sync validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the collection item being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object property to validate.</typeparam>
    /// <param name="builder">The validation builder for the collection item property.</param>
    /// <param name="validator">The sync validator instance to use for each item.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.EnsureEach(x => x.Orders)
    ///     .ValidateWith(new OrderValidator());
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget>(
        this IAsyncEnsureEachBuilderType<TRequest, TTarget> builder,
        IValidator<TTarget> validator)
        where TRequest : class
        where TTarget : class
        => throw new NotImplementedException("Never gets called");

    /// <summary>
    /// Delegates validation of a nested object to a sync validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the parent object being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object to validate.</typeparam>
    /// <param name="builder">The validation builder for the nested object property.</param>
    /// <param name="validator">The sync validator instance to use for the nested object.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Address)
    ///     .ValidateWith(addressValidator);
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget>(
        this IAsyncEnsureBuilderType<TRequest, TTarget> builder,
        IValidator<TTarget> validator)
        where TRequest : class
        where TTarget : class
        => throw new NotImplementedException("Never gets called");

    /// <summary>
    /// Delegates validation of each item in a collection to an async validator.
    /// The generator will call ValidateToListAsync on the async validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the collection item being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object property to validate.</typeparam>
    /// <param name="builder">The validation builder for the collection item property.</param>
    /// <param name="validator">The async validator instance to use for each item.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.EnsureEach(x => x.Orders)
    ///     .ValidateWith(new AsyncOrderValidator());
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget>(
        this IAsyncEnsureEachBuilderType<TRequest, TTarget> builder,
        IAsyncValidator<TTarget> validator)
        where TRequest : class
        where TTarget : class
        => throw new NotImplementedException("Never gets called");

    /// <summary>
    /// Delegates validation of a nested object to an async validator.
    /// The generator will call ValidateToListAsync on the async validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the parent object being validated.</typeparam>
    /// <typeparam name="TTarget">The type of the nested object to validate.</typeparam>
    /// <param name="builder">The validation builder for the nested object property.</param>
    /// <param name="validator">The async validator instance to use for the nested object.</param>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <example>
    /// <code>
    /// builder.Ensure(x => x.Address)
    ///     .ValidateWith(new AsyncAddressValidator());
    /// </code>
    /// </example>
    public static IAsyncValidateWithBuilderType<TRequest, TTarget> ValidateWith<TRequest, TTarget>(
        this IAsyncEnsureBuilderType<TRequest, TTarget> builder,
        IAsyncValidator<TTarget> validator)
        where TRequest : class
        where TTarget : class
        => throw new NotImplementedException("Never gets called");
}