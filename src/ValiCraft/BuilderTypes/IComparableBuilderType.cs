namespace ValiCraft.BuilderTypes;

/// <summary>
/// A builder type for properties that implement IComparable.
/// Comparison rules like IsGreaterThan, IsBetween, etc. extend this interface.
/// </summary>
public interface IComparableBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class
    where TTarget : IComparable<TTarget>;
