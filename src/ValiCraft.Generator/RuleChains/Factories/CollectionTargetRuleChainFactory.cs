using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionTargetRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        // Resolve the element type from the EnsureEach method's TTarget generic argument.
        var elementTypeInfo = context.Invocation.GetElementTypeInfo(context.GeneratorContext);
        if (elementTypeInfo is null)
        {
            return null;
        }

        // Skip the EnsureEach method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            context.IsAsyncValidator, context.InvocationChain.Skip(1), context.Diagnostics, context.GeneratorContext);
        if (rules is null)
        {
            return null;
        }

        return new CollectionTargetRuleChain(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count,
            context.Invocation.GetOnFailureModeFromSyntax(),
            elementTypeInfo,
            rules.ToEquatableImmutableArray());
    }
}
