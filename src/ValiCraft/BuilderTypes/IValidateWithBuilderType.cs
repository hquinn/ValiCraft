namespace ValiCraft.BuilderTypes;

public interface IValidateWithBuilderType<TRequest, TProperty> : IBuilderType<TRequest, TProperty>
    where TRequest : class;