namespace ValiCraft.Abstractions.BuilderTypes;

public interface IUseValidatorBuilderType<TRequest, TProperty>  : IBuilderType<TRequest, TProperty>
    where TRequest : class
{
}