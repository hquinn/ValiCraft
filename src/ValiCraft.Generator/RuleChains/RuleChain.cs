using System.Text;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains;

public abstract record RuleChain(
    int Depth,
    int NumberOfRules,
    OnFailureMode? FailureMode)
{
    private const int TabIndexSize = 4;
    private const int BaseLineIndent = TabIndexSize * 3;
    
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
        return Depth switch
        {
            0 => "request",
            1 => "element",
            2 => "subElement",
            _ => $"subElement{Depth - 1}"
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