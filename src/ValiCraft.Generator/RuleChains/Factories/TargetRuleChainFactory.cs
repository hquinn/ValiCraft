using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetRuleChainFactory(bool isCollection = false) : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        CollectionConfig? resolvedCollection = null;
        if (isCollection)
        {
            // Resolve the element type from the EnsureEach method's TTarget generic argument.
            var elementTypeInfo = context.Invocation.GetElementTypeInfo(context.GeneratorContext);
            if (elementTypeInfo is null)
            {
                return null;
            }

            resolvedCollection = new CollectionConfig(elementTypeInfo);
        }

        // Skip the Ensure/EnsureEach method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            context.IsAsyncValidator, context.InvocationChain.Skip(1), context.Diagnostics, context.GeneratorContext);
        if (rules is null)
        {
            return null;
        }

        // Now that we have all the rules in the chain, we can now create the rule chain
        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count,
            context.Invocation?.GetOnFailureModeFromSyntax());

        return new TargetRuleChain(
            config,
            rules.ToEquatableImmutableArray(),
            resolvedCollection);
    }
}
