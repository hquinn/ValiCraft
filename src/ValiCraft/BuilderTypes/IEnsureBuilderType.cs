namespace ValiCraft.BuilderTypes;

public interface IEnsureBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class;