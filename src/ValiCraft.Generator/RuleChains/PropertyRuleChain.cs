using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record PropertyRuleChain(
    ValidationTarget Target,
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules) : RuleChain(Target, Depth, NumberOfRules, FailureMode)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        var rules = new List<Rule>(Rules.Count);

        foreach (var rule in Rules)
        {
            if (rule.SemanticMode is SemanticMode.WeakSemanticMode)
            {
                var matchedValidationRule = rule.MapToValidationRule(Target!, validRules);

                if (matchedValidationRule is null)
                {
                    var diagnostics =
                        DefinedDiagnostics.UnrecognizableRuleInvocation(rule.Location.ToLocation());
                    context.ReportDiagnostic(diagnostics.CreateDiagnostic());

                    linkedRuleChain = this;
                    return false;
                }

                rules.Add(rule.EnrichRuleFromValidationRule(matchedValidationRule));
                continue;
            }
            
            rules.Add(rule);
        }

        linkedRuleChain = this with
        {
            Rules = rules.ToEquatableImmutableArray()
        };
        
        return true;
    }

    public override bool NeedsGotoLabels()
    {
        // Property Rule Chains themselves don't require the need for goto labels,
        // but they will need to implement goto labels if other rule chains from its parent rule chain do.
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = new List<string>(NumberOfRules);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                GetRequestParameterName(),
                GetIndent(),
                Target!,
                context));
            context.DecrementCountdown();
        }
        
        return string.Join("\r\n", ruleCodes);
    }
}