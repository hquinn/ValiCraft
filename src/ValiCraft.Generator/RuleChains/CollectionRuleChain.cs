using System.Collections.Generic;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CollectionRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ItemRuleChains) : CollectionItemRuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (ItemRuleChains.Count == 0)
        {
            return string.Empty;
        }

        var index = $"index{context.Counter}";
        var itemRuleChainCodes = new List<string>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            itemRuleChainCodes.Add(itemRuleChain.GenerateCode(context));
        }

        return GenerateForEachLoop(index, string.Join("\r\n", itemRuleChainCodes));
    }
}