using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public abstract record RuleChain(
    ValidationTarget? Target,
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode)
{
    private const int TabIndexSize = 4;
    private const int BaseLineIndent = TabIndexSize * 3;

    public bool TryLinkRuleChain(
        List<RuleChain> ruleChains,
        ValidationRule[] validRules,
        SourceProductionContext context)
    {
        if (!TryLinkRuleChain(validRules, context, out var ruleChain))
        {
            return false;
        }
        
        ruleChains.Add(ruleChain);
        return true;
    }

    protected abstract bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain);
    
    public string GenerateCode(RuleChainContext context)
    {
        var code = new StringBuilder();

        var contextToUse = DetermineRuleChainContext(context);
        code.Append(HandleCodeGeneration(contextToUse));
        
        AppendGotoLabelIfNeeded(code, contextToUse, context);
        
        return code.ToString();
    }

    public abstract bool NeedsGotoLabels();
    
    protected abstract string HandleCodeGeneration(RuleChainContext context);

    public string GetRequestParameterName()
    {
        return CalculateRequestParameterName(Depth);
    }

    public string GetItemRequestParameterName()
    {
        return CalculateRequestParameterName(Depth + 1);
    }

    private static string CalculateRequestParameterName(int depth)
    {
        return depth switch
        {
            0 => "request",
            1 => "element",
            2 => "subElement",
            _ => $"subElement{depth - 1}"
        };
    }

    protected string GetIndent()
    {
        return new string(' ', BaseLineIndent + Depth * TabIndexSize);
    }

    private RuleChainContext DetermineRuleChainContext(RuleChainContext context)
    {
        return context.ParentFailureMode is null
            ? HandleNoParentFailureMode(context)
            : HandleWithParentFailureMode(context);
    }

    private RuleChainContext HandleNoParentFailureMode(RuleChainContext context)
    {
        return FailureMode is OnFailureMode.Halt 
            ? context.CreateHaltContext(NeedsGotoLabels())
            : context;
    }

    private RuleChainContext HandleWithParentFailureMode(RuleChainContext context)
    {
        return FailureMode switch
        {
            OnFailureMode.Halt => HandleHaltFailureMode(context),
            OnFailureMode.Continue => HandleContinueFailureMode(context),
            _ => context
        };
    }

    private RuleChainContext HandleHaltFailureMode(RuleChainContext context)
    {
        return context.ParentFailureMode is OnFailureMode.Continue
            ? context.CreateHaltContext(NeedsGotoLabels())
            : context;
    }

    private RuleChainContext HandleContinueFailureMode(RuleChainContext context)
    {
        return context.ParentFailureMode is OnFailureMode.Continue
            ? context
            : context.CreateContinueContext();
    }

    private void AppendGotoLabelIfNeeded(
        StringBuilder code,
        RuleChainContext contextToUse,
        RuleChainContext parentContext)
    {
        if (parentContext.HaltLabel is null && contextToUse.HaltLabel is not null)
        {
            var indent = GetIndent();
            code.Append($"\r\n\r\n{indent}{contextToUse.HaltLabel}:");
        }
    }
}