using System.Linq;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            context.IsAsyncValidator, context.Invocation, KnownNames.Methods.EnsureEach,
            context.Depth + 1, IndentModel.CreateChild(context.Indent), context.Diagnostics, context.GeneratorContext);
        if (ruleChains is null)
        {
            return null;
        }

        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            ruleChains.Sum(x => x.Config.NumberOfRules),
            context.Invocation.GetOnFailureModeFromSyntax());

        return new CollectionRuleChain(
            config,
            ruleChains.ToEquatableImmutableArray());
    }
}