using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.RuleChains;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record Validator(
    ClassInfo Class,
    string RequestTypeName,
    EquatableArray<RuleChain> RuleChains,
    EquatableArray<string> UsingDirectives)
{
    public bool TryLinkWeakSemanticRules(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out Validator? validator)
    {
        var ruleChains = new List<RuleChain>(RuleChains.Count);
        
        foreach (var ruleChain in RuleChains)
        {
            if (!ruleChain.TryLinkRuleChain(ruleChains, validRules, context))
            {
                validator = this;
                return false;
            }
        }

        validator = this with
        {
            RuleChains = ruleChains.ToEquatableImmutableArray()
        };
        
        return true;
    }
}