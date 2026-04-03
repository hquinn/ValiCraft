using System.Linq;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record WithOnFailureRuleChain(
    RuleChainConfig Config,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        return ChildRuleChains.Any(x => x.NeedsGotoLabels());
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var code = string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)));
        context.ResetIfElseMode();

        return code;
    }
}