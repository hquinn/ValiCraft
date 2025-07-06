using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record PropertyRuleChain(
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    ArgumentInfo Property,
    EquatableArray<Rule> Rules) : RuleChain(Depth, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // Property Rule Chains themselves don't require the need for goto labels,
        // but they will need to implement goto labels if other rule chains from its parent rule chain do.
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = new List<string>(NumberOfRules);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                GetRequestParameterName(),
                GetIndent(),
                Property,
                context));
            context.DecrementCountdown();
        }
        
        return string.Join("\r\n", ruleCodes);
    }
}