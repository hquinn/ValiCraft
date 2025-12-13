namespace ValiCraft.BuilderTypes;

/// <summary>
/// A builder type for collection properties.
/// Collection validation rules like HasMinCount, HasItems, etc. extend this interface
/// to ensure they only appear in IntelliSense for collection properties.
/// </summary>
public interface ICollectionBuilderType<TRequest, TItem> : IBuilderType<TRequest, IEnumerable<TItem>?>
    where TRequest : class;
