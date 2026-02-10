namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Builder type for validating each item in a collection.
/// Used internally by <c>EnsureEach()</c> to provide validation context for collection items.
/// </summary>
/// <typeparam name="TRequest">The type of the collection item being validated.</typeparam>
/// <typeparam name="TTarget">The type of the property on each collection item.</typeparam>
/// <remarks>
/// This builder type enables nested validation within collections, where each item
/// is validated according to the rules defined in the builder callback.
/// </remarks>
public interface IAsyncEnsureEachBuilderType<TRequest, TTarget> 
    where TRequest : class
{
    /// <summary>
    /// Delegates validation of each item in the collection to a static validator synchronously.
    /// The validator must implement <see cref="IStaticValidator{T}"/> where T is the target type.
    /// </summary>
    /// <typeparam name="TValidator">The static validator type to use for validation.</typeparam>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <remarks>
    /// This method is only valid when <typeparamref name="TTarget"/> is a reference type.
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticValidator{T}"/> for the correct target type.
    /// </remarks>
    IAsyncStaticValidateBuilderType<TRequest, TTarget> Validate<TValidator>();

    /// <summary>
    /// Delegates validation of each item in the collection to a static async validator.
    /// The validator must implement <see cref="IStaticAsyncValidator{T}"/> where T is the target type.
    /// </summary>
    /// <typeparam name="TValidator">The static async validator type to use for validation.</typeparam>
    /// <returns>A builder for chaining additional validation rules.</returns>
    /// <remarks>
    /// This method is only valid when <typeparamref name="TTarget"/> is a reference type.
    /// The source generator will verify that <typeparamref name="TValidator"/> implements
    /// <see cref="IStaticAsyncValidator{T}"/> for the correct target type.
    /// </remarks>
    IAsyncStaticValidateBuilderType<TRequest, TTarget> ValidateAsync<TValidator>();
}