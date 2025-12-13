namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned after applying <c>ValidateWith()</c> to delegate validation to another validator.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The type of the nested object being validated by the delegate validator.</typeparam>
/// <remarks>
/// This builder type allows chaining additional validation rules after delegating
/// to a nested validator. It supports validating complex object graphs with
/// multiple validators.
/// </remarks>
public interface IValidateWithBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class;