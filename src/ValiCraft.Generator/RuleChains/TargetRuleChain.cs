using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules,
    bool WhenNotNull = false) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        var rules = new List<Rule>(Rules.Count);

        foreach (var rule in Rules)
        {
            if (rule.EnrichRule(Target!, validRules, context) is not { } linkedRule)
            {
                linkedRuleChain = this;
                return false;
            }
            rules.Add(linkedRule);
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
                Indent,
                Object,
                Target!,
                context));
            context.DecrementCountdown();
        }
        
        var generatedCode = string.Join("\r\n", ruleCodes);
        
        // If WhenNotNull is set, wrap the validation code in a null check
        if (WhenNotNull)
        {
            var targetAccess = string.Format(Target!.AccessorExpressionFormat, GetRequestParameterName());
            generatedCode = Indent + "if (" + targetAccess + " is not null)\r\n" +
                            Indent + "{\r\n" +
                            generatedCode + "\r\n" +
                            Indent + "}";
        }
        
        return generatedCode;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}
