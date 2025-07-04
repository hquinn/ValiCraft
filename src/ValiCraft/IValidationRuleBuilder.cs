using System.Linq.Expressions;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    IEnsureBuilderType<TRequest, TProperty> Ensure<TProperty>(Expression<Func<TRequest, TProperty>> selector);
    IEnsureEachBuilderType<TRequest, TProperty> EnsureEach<TProperty>(
        Expression<Func<TRequest, IEnumerable<TProperty>>> selector,
        Action<IValidationRuleBuilder<TProperty>> rules) where TProperty : class;
}