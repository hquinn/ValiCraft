using System.Text;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public abstract record RuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget? Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode)
{
    public string GenerateCode(RuleChainContext context)
    {
        var code = new StringBuilder();

        var contextToUse = DetermineRuleChainContext(context);
        var currentTargetPath = contextToUse.TargetPath;
        
        contextToUse.TargetPath = GetTargetPath(contextToUse);
        code.Append(HandleCodeGeneration(contextToUse));
        contextToUse.TargetPath = currentTargetPath;
        
        AppendGotoLabelIfNeeded(code, contextToUse, context);
        
        return code.ToString();
    }

    public abstract bool NeedsGotoLabels();
    
    protected abstract string HandleCodeGeneration(RuleChainContext context);
    
    protected abstract string GetTargetPath(RuleChainContext context);
    
    protected string GetRequestParameterName()
    {
        return CalculateRequestParameterName(Depth);
    }

    protected string GetItemRequestParameterName()
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
            code.Append($"\r\n\r\n{Indent}{contextToUse.HaltLabel}:");
        }
    }
}