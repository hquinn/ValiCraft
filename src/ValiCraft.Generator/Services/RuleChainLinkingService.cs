using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Services;

public static class RuleChainLinkingService
{
    public static bool TryLinkWeakSemanticRules(
        ProviderResult<Validator> validatorResult,
        ValidationRule[] validRules,
        SourceProductionContext context,
        out Validator validator)
    {
        validator = validatorResult.Value!;
        var ruleChains = new List<RuleChain>(validator.RuleChains.Count);
        
        foreach (var ruleChain in validator.RuleChains)
        {
            if (!TryLinkRuleChain(ruleChains, validRules, ruleChain, context))
            {
                return false;
            }
        }

        validator = validator with
        {
            RuleChains = ruleChains.ToEquatableImmutableArray()
        };
        
        return true;
    }
    
    private static bool TryLinkRuleChain(
        List<RuleChain> itemRuleChains,
        ValidationRule[] validRules,
        RuleChain itemRuleChain,
        SourceProductionContext context)
    {
        if (itemRuleChain is PropertyRuleChain itemPropertyRuleChain)
        {
            if (!TryLinkPropertyRuleChain(itemPropertyRuleChain, validRules, context, out var linkedItemPropertyRuleChain))
            {
                return false;
            }
            
            itemRuleChains.Add(linkedItemPropertyRuleChain);
        }
        else if (itemRuleChain is CollectionRuleChain itemCollectionRuleChain)
        {
            if (!TryLinkCollectionRuleChain(itemCollectionRuleChain, validRules, context, out var linkedItemCollectionRuleChain))
            {
                return false;
            }
            
            itemRuleChains.Add(linkedItemCollectionRuleChain);
        }
        else if (itemRuleChain is ValidateWithRuleChain itemValidateWithRuleChain)
        {
            itemRuleChains.Add(itemValidateWithRuleChain);
        }

        return true;
    }
    
    private static bool TryLinkPropertyRuleChain(
        PropertyRuleChain propertyRuleChain,
        ValidationRule[] validRules,
        SourceProductionContext context,
        out PropertyRuleChain linkedPropertyRuleChain)
    {
        linkedPropertyRuleChain = propertyRuleChain;
        var rules = new List<Rule>(propertyRuleChain.Rules.Count);

        foreach (var rule in propertyRuleChain.Rules)
        {
            if (rule.SemanticMode is SemanticMode.WeakSemanticMode)
            {
                var matchedValidationRule = rule.MapToValidationRule(validRules);

                if (matchedValidationRule is null)
                {
                    var diagnostics =
                        DefinedDiagnostics.UnrecognizableRuleInvocation(rule.Location.ToLocation());
                    context.ReportDiagnostic(diagnostics.CreateDiagnostic());

                    return false;
                }

                rules.Add(rule.EnrichRuleFromValidationRule(matchedValidationRule));
                continue;
            }
            
            rules.Add(rule);
        }

        linkedPropertyRuleChain = linkedPropertyRuleChain with
        {
            Rules = rules.ToEquatableImmutableArray()
        };
        
        return true;
    }
    
    private static bool TryLinkCollectionRuleChain(
        CollectionRuleChain collectionRuleChain,
        ValidationRule[] validRules,
        SourceProductionContext context,
        out CollectionRuleChain linkedCollectionRuleChain)
    {
        linkedCollectionRuleChain = collectionRuleChain;
        var itemRuleChains = new List<RuleChain>(collectionRuleChain.ItemRuleChains.Count);

        foreach (var itemRuleChain in collectionRuleChain.ItemRuleChains)
        {
            if (!TryLinkRuleChain(itemRuleChains, validRules, itemRuleChain, context))
            {
                return false;
            }
        }

        linkedCollectionRuleChain = linkedCollectionRuleChain with
        {
            ItemRuleChains = itemRuleChains.ToEquatableImmutableArray()
        };
        
        return true;
    }
}