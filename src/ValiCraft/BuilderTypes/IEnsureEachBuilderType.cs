namespace ValiCraft.BuilderTypes;

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
public interface IEnsureEachBuilderType<TRequest, TTarget> 
    where TRequest : class;