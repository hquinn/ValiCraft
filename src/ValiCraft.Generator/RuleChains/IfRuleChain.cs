using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record IfRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    IfConditionModel IfCondition,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(IsAsync, Object, null, Depth, Indent, NumberOfRules, null)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        var childRuleChains = new List<RuleChain>(ChildRuleChains.Count);

        foreach (var itemRuleChain in ChildRuleChains)
        {
            if (!itemRuleChain.TryLinkRuleChain(childRuleChains, validRules, context))
            {
                linkedRuleChain = this;
                return false;
            }
        }

        linkedRuleChain = this with
        {
            ChildRuleChains = childRuleChains.ToEquatableImmutableArray()
        };
        
        return true;
    }

    public override bool NeedsGotoLabels()
    {
        return ChildRuleChains.Any(x => x.NeedsGotoLabels());
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (ChildRuleChains.Count == 0)
        {
            return string.Empty;
        }
        
        var code = $$"""
                   {{IfCondition.GenerateIfBlock(Object, GetRequestParameterName(), Indent, context)}}
                   {{Indent}}{
                   {{string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)))}}
                   {{Indent}}}
                   """;

        context.ResetIfElseMode();
        
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }
}