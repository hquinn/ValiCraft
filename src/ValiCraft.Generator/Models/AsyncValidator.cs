using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.RuleChains;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

/// <summary>
/// Represents an async validator class being processed by the source generator.
/// </summary>
public record AsyncValidator(
    ClassInfo Class,
    string RequestTypeName,
    EquatableArray<RuleChain> RuleChains,
    EquatableArray<string> UsingDirectives)
{
    public bool TryLinkWeakSemanticRules(
        AsyncValidationRule[] validRules,
        ValidationRule[] syncRules,
        SourceProductionContext context,
        out AsyncValidator? validator)
    {
        var ruleChains = new List<RuleChain>(RuleChains.Count);
        
        foreach (var ruleChain in RuleChains)
        {
            if (!ruleChain.TryLinkAsyncRuleChain(ruleChains, validRules, syncRules, context))
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
