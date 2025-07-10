namespace ValiCraft.BuilderTypes;

public interface IEnsureEachBuilderType<TRequest, TTarget> 
    where TRequest : class
    where TTarget : class;