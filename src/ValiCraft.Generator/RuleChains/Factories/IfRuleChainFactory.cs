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

        return new IfRuleChain(
            context.IsAsyncValidator,
            context.Object,
            context.Depth,
            context.Indent,
            ruleChains.Sum(x => x.NumberOfRules),
            ifConditionArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}