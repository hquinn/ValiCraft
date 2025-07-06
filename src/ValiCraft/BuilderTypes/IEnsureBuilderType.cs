namespace ValiCraft.BuilderTypes;

public interface IEnsureBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class;