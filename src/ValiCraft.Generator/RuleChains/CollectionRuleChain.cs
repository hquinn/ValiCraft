using System.Collections.Generic;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CollectionRuleChain(
    RuleChainConfig Config,
    EquatableArray<RuleChain> ItemRuleChains) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        // Loops have no reliable way (besides break and return) to exit loops early
        return true;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Config.Target!.TargetPath.Value}[{{{indexer}}}].";
    }

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