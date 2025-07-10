namespace ValiCraft.BuilderTypes;

public interface IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IValidationRuleBuilderType<TRequest, TTarget> Is<TValidationRule>()
        where TValidationRule : IValidationRule<TTarget>;

    IValidationRuleBuilderType<TRequest, TTarget> Is<TValidationRule, TParam1>(TParam1 param1)
        where TValidationRule : IValidationRule<TTarget, TParam1>;

    IValidationRuleBuilderType<TRequest, TTarget> Is<TValidationRule, TParam1, TParam2>(
        TParam1 param1,
        TParam2 param2)
        where TValidationRule : IValidationRule<TTarget, TParam1, TParam2>;

    IValidationRuleBuilderType<TRequest, TTarget> Is<TValidationRule, TParam1, TParam2, TParam3>(
        TParam1 param1,
        TParam2 param2,
        TParam3 param3)
        where TValidationRule : IValidationRule<TTarget, TParam1, TParam2, TParam3>;

    IValidationRuleBuilderType<TRequest, TTarget> Is<TValidationRule, TParam1, TParam2, TParam3, TParam4>(
        TParam1 param1,
        TParam2 param2,
        TParam3 param3,
        TParam4 param4)
        where TValidationRule : IValidationRule<TTarget, TParam1, TParam2, TParam3, TParam4>;

    IValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);
}