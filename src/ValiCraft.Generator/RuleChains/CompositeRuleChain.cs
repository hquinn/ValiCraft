using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record CompositeRuleChain(
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(null, Depth, NumberOfRules, FailureMode)
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
        var code = string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)));
        context.ResetIfElseMode();
        
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }
}