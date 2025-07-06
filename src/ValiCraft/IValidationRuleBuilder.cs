using System.Linq.Expressions;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    IEnsureBuilderType<TRequest, TProperty> Ensure<TProperty>(
        Expression<Func<TRequest, TProperty>> selector,
        OnFailureMode? failureMode = null);
    IEnsureEachBuilderType<TRequest, TProperty> EnsureEach<TProperty>(
        Expression<Func<TRequest, IEnumerable<TProperty>>> selector,
        OnFailureMode? failureMode = null) where TProperty : class;
    void EnsureEach<TProperty>(
        Expression<Func<TRequest, IEnumerable<TProperty>>> selector,
        Action<IValidationRuleBuilder<TProperty>> rules) where TProperty : class;
    void EnsureEach<TProperty>(
        Expression<Func<TRequest, IEnumerable<TProperty>>> selector,
        OnFailureMode failureMode,
        Action<IValidationRuleBuilder<TProperty>> rules) where TProperty : class;
    void WithOnFailure(OnFailureMode failureMode, Action<IValidationRuleBuilder<TRequest>> rules);
}