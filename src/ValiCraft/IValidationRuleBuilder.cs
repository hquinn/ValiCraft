using System.Linq.Expressions;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    IEnsureBuilderType<TRequest, TProperty> Ensure<TProperty>(Expression<Func<TRequest, TProperty>> selector);
}