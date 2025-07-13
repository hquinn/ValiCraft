namespace ValiCraft.BuilderTypes;

public interface IValidateWithBuilderType<TRequest, TTarget> : IBuilderType<TRequest, TTarget>
    where TRequest : class;