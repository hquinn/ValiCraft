using System.Linq.Expressions;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    IEnsureBuilderType<TRequest, TTarget> Ensure<TTarget>(
        Expression<Func<TRequest, TTarget>> selector,
        OnFailureMode? failureMode = null);
    IEnsureEachBuilderType<TRequest, TTarget> EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        OnFailureMode? failureMode = null) where TTarget : class;
    void EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        Action<IValidationRuleBuilder<TTarget>> rules) where TTarget : class;
    void EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        OnFailureMode failureMode,
        Action<IValidationRuleBuilder<TTarget>> rules) where TTarget : class;
    void WithOnFailure(OnFailureMode failureMode, Action<IValidationRuleBuilder<TRequest>> rules);
    void If(Func<TRequest, bool> condition, Action<IValidationRuleBuilder<TRequest>> rules);
}