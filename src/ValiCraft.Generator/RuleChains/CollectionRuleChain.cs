using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CollectionRuleChain(
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    ArgumentInfo Property,
    EquatableArray<RuleChain> ItemRuleChains) : RuleChain(Depth, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // Loops have no reliable way (besides break and return) to exit loops early
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (ItemRuleChains.Count == 0)
        {
            return string.Empty;
        }
        
        var itemRuleChainCodes = new List<string>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            itemRuleChainCodes.Add(itemRuleChain.GenerateCode(context));
        }

        var indent = GetIndent();
        var requestName = GetRequestParameterName();
        var itemRequestName = ItemRuleChains.First().GetRequestParameterName();
        var ruleChainCodes = string.Join("\r\n", itemRuleChainCodes);
        
        return $$"""
               {{indent}}foreach (var {{itemRequestName}} in {{requestName}}.{{Property.Value}})
               {{indent}}{
               {{ruleChainCodes}}
               {{indent}}}
               """;
    }
}