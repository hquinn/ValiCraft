namespace ValiCraft.BuilderTypes;

public interface IEnsureEachBuilderType<TRequest, TProperty> 
    where TRequest : class
    where TProperty : class;