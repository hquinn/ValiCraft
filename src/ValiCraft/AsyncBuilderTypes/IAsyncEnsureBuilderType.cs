namespace ValiCraft.AsyncBuilderTypes;

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
public interface IAsyncEnsureBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class;