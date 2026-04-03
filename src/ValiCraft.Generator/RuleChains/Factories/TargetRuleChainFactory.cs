using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        // Skip the Ensure method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            context.IsAsyncValidator, context.InvocationChain.Skip(1), context.Diagnostics, context.GeneratorContext);
        if (rules is null)
        {
            return null;
        }

        // Now that we have all the rules in the chain, we can now create the rule chain
        return new TargetRuleChain(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count,
            context.Invocation?.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray());
    }
}
