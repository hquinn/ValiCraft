using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CollectionRuleChain(
    ValidationTarget Target,
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ItemRuleChains) : RuleChain(Target, Depth, NumberOfRules, FailureMode)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        var itemRuleChains = new List<RuleChain>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            if (!itemRuleChain.TryLinkRuleChain(itemRuleChains, validRules, context))
            {
                linkedRuleChain = this;
                return false;
            }
        }

        linkedRuleChain = this with
        {
            ItemRuleChains = itemRuleChains.ToEquatableImmutableArray()
        };
        
        return true;
    }

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

        var index = $"index{context.Counter}";
        var itemRuleChainCodes = new List<string>(ItemRuleChains.Count);

        foreach (var itemRuleChain in ItemRuleChains)
        {
            itemRuleChainCodes.Add(itemRuleChain.GenerateCode(context));
        }

        var indent = GetIndent();
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        
        var itemRequestName = GetItemRequestParameterName();

        var ruleChainCodes = string.Join("\r\n", itemRuleChainCodes);
        
        return $$"""
               {{indent}}var {{index}} = 0;
               {{indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
               {{indent}}{
               {{ruleChainCodes}}
               {{indent}}    {{index}}++;
               {{indent}}}
               """;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}