using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record CollectionRuleChain(
    ArgumentInfo Property,
    EquatableArray<RuleChain> ItemRuleChains,
    int NumberOfRules) : RuleChain(NumberOfRules)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
    {
        var itemRuleChainCodes = new List<string>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            itemRuleChainCodes.Add(itemRuleChain.GenerateCodeForRuleChain(ref assignedErrorsCount));
        }
        
        return string.Join("\r\n", itemRuleChainCodes);
    }
}