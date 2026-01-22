using System.Linq;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record WithOnFailureRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(IsAsync, Object, null, Depth, Indent, NumberOfRules, FailureMode)
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

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }
}