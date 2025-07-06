using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record CollectionRuleChain(
    ArgumentInfo Property,
    int Depth,
    int NumberOfRules,
    EquatableArray<RuleChain> ItemRuleChains) : RuleChain(Property, Depth, NumberOfRules)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
    {
        if (ItemRuleChains.Count == 0)
        {
            return string.Empty;
        }
        
        var itemRuleChainCodes = new List<string>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            itemRuleChainCodes.Add(itemRuleChain.GenerateCodeForRuleChain(ref assignedErrorsCount));
        }

        var indent = GetIndent();
        var requestName = GetRequestParameterName();
        var itemRequestName = ItemRuleChains.First().GetRequestParameterName();
        var ruleChainCodes = string.Join("\r\n\r\n", itemRuleChainCodes);
        
        return $$"""
               {{indent}}foreach (var {{itemRequestName}} in {{requestName}}.{{Property.Value}})
               {{indent}}{
               {{ruleChainCodes}}
               {{indent}}}
               """;
    }
}