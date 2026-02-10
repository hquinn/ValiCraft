namespace ValiCraft.AsyncBuilderTypes;

/// <summary>
/// Builder type returned after applying <c>Validate&lt;TValidator&gt;()</c> or <c>ValidateAsync&lt;TValidator&gt;()</c> 
/// to delegate validation to a static validator in an async context.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the nested object being validated by the static validator.</typeparam>
/// <remarks>
/// This builder type allows chaining additional validation rules after delegating
/// to a static nested validator. It supports validating complex object graphs with
/// multiple validators without requiring instance creation.
/// </remarks>
public interface IAsyncStaticValidateBuilderType<TRequest, TTarget> : IAsyncBuilderType<TRequest, TTarget>
    where TRequest : class;
