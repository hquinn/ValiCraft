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

        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            null,
            context.Depth,
            context.Indent,
            ruleChains.Sum(x => x.Config.NumberOfRules),
            onFailureArgument);

        return new WithOnFailureRuleChain(
            config,
            ruleChains.ToEquatableImmutableArray());
    }
}