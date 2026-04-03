using System.Collections.Generic;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{

    public override bool NeedsGotoLabels()
    {
        // Property Rule Chains themselves don't require the need for goto labels,
        // but they will need to implement goto labels if other rule chains from its parent rule chain do.
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Indent, Object, Target!, context);

        return string.Join("\r\n", ruleCodes);
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}
