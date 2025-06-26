namespace ValiCraft.Abstractions;

public abstract class Validator<TRequest> where TRequest : class
{
    /// <summary>
    /// Define validation rules
    /// </summary>
    protected abstract void DefineRules(IValidationRuleBuilder<TRequest> builder);
}