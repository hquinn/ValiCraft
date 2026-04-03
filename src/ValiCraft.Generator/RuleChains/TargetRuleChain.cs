using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetRuleChain(
    RuleChainConfig Config,
    EquatableArray<Rule> Rules) : RuleChain(Config)
{

    public override bool NeedsGotoLabels()
    {
        // Property Rule Chains themselves don't require the need for goto labels,
        // but they will need to implement goto labels if other rule chains from its parent rule chain do.
        return false;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return BuildTargetPath(context.TargetPath, Config.Target!.TargetPath.Value, false, context.Counter);
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Config.Indent, Config.Object, Config.Target!, context);

        return string.Join("\r\n", ruleCodes);
    }

}
