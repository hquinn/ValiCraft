namespace ValiCraft.BuilderTypes;

public interface IBuilderType<TRequest, TProperty>
    where TRequest : class
{
    IValidationRuleBuilderType<TRequest, TProperty> Is<TValidationRule>()
        where TValidationRule : IValidationRule<TProperty>;
    
    IValidationRuleBuilderType<TRequest, TProperty> Is<TValidationRule, TParam1>(TParam1 param1)
        where TValidationRule : IValidationRule<TProperty, TParam1>;
    
    IValidationRuleBuilderType<TRequest, TProperty> Is<TValidationRule, TParam1, TParam2>(
        TParam1 param1,
        TParam2 param2)
        where TValidationRule : IValidationRule<TProperty, TParam1, TParam2>;
    
    IValidationRuleBuilderType<TRequest, TProperty> Is<TValidationRule, TParam1, TParam2, TParam3>(
        TParam1 param1,
        TParam2 param2,
        TParam3 param3)
        where TValidationRule : IValidationRule<TProperty, TParam1, TParam2, TParam3>;
    
    IValidationRuleBuilderType<TRequest, TProperty> Is<TValidationRule, TParam1, TParam2, TParam3, TParam4>(
        TParam1 param1,
        TParam2 param2,
        TParam3 param3,
        TParam4 param4)
        where TValidationRule : IValidationRule<TProperty, TParam1, TParam2, TParam3, TParam4>;
}