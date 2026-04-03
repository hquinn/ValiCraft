using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class WithOnFailureRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var onFailureArgument = context.Invocation.GetOnFailureModeFromSyntax();

        if (onFailureArgument is null)
        {
            return null;
        }

        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            context.IsAsyncValidator, context.Invocation, KnownNames.Methods.WithOnFailure,
            context.Depth, context.Indent, context.Diagnostics, context.GeneratorContext);
        if (ruleChains is null)
        {
            return null;
        }

        return new WithOnFailureRuleChain(
            context.IsAsyncValidator,
            context.Object,
            context.Depth,
            context.Indent,
            ruleChains.Sum(x => x.NumberOfRules),
            onFailureArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}