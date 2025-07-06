using System.Linq;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CompositeRuleChain(
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(Depth, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        return ChildRuleChains.Any(x => x.NeedsGotoLabels());
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var code = string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)));
        context.ResetIfElseMode();
        
        return code;
    }
}