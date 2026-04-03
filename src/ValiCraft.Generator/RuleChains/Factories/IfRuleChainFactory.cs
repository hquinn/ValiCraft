using System.Linq;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class IfRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var ifConditionArgument = IfConditionFactory.Create(context.Invocation, true);

        // No need for diagnostics, as having a null condition in this case isn't legal C#
        if (ifConditionArgument is null)
        {
            return null;
        }

        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            context.IsAsyncValidator, context.Invocation, KnownNames.Methods.If,
            context.Depth, IndentModel.CreateChild(context.Indent), context.Diagnostics, context.GeneratorContext);
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
            null);

        return new IfRuleChain(
            config,
            ifConditionArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}