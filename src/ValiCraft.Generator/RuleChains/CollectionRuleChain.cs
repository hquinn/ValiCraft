using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CollectionRuleChain(
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ItemRuleChains) : RuleChain(Object, Target, Depth, Indent, NumberOfRules, FailureMode)
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

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        
        var itemRequestName = GetItemRequestParameterName();

        var ruleChainCodes = string.Join("\r\n", itemRuleChainCodes);
        
        return $$"""
               {{Indent}}var {{index}} = 0;
               {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
               {{Indent}}{
               {{ruleChainCodes}}
               {{Indent}}    {{index}}++;
               {{Indent}}}
               """;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}