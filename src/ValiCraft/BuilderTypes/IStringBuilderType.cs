namespace ValiCraft.BuilderTypes;

/// <summary>
/// A builder type specifically for string properties.
/// String validation rules like IsEmailAddress, HasMinLength, etc. extend this interface
/// to ensure they only appear in IntelliSense for string properties.
/// </summary>
public interface IStringBuilderType<TRequest> : IBuilderType<TRequest, string?>
    where TRequest : class;
