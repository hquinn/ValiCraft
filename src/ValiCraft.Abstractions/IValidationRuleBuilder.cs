using System.Linq.Expressions;
using ValiCraft.Abstractions.BuilderTypes;

namespace ValiCraft.Abstractions;

public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    IEnsureBuilderType<TRequest, TProperty> Ensure<TProperty>(Expression<Func<TRequest, TProperty>> selector);
}