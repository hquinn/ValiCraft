namespace ValiCraft.BuilderTypes;

/// <summary>
/// A builder type specifically for DateTime properties.
/// DateTime-specific validation rules like IsInFuture, IsInPast, etc. extend this interface
/// to ensure they only appear in IntelliSense for DateTime properties.
/// </summary>
public interface IDateTimeBuilderType<TRequest> : IBuilderType<TRequest, DateTime>
    where TRequest : class;

/// <summary>
/// A builder type specifically for nullable DateTime properties.
/// </summary>
public interface INullableDateTimeBuilderType<TRequest> : IBuilderType<TRequest, DateTime?>
    where TRequest : class;
