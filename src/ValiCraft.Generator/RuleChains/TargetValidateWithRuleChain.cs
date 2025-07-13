using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetValidateWithRuleChain(
    ValidationTarget Target,
    int Depth,
    OnFailureMode? FailureMode,
    string ValidatorExpression) : RuleChain(Target, Depth, 1, FailureMode)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        linkedRuleChain = this;

        return true;
    }

    public override bool NeedsGotoLabels()
    {
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var indent = GetIndent();
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        
        // A little hacky with the assigned errors, but it's a quick fix to get uniqueness.
        var code = $$"""
                     {{indent}}var errors{{context.Counter}} = {{ValidatorExpression}}.ValidateToList({{requestAccessor}}, $"{inheritedTargetPath}{{Target.TargetPath.Value}}.");
                     {{indent}}{{GetIfElseIfKeyword(context)}} (errors{{context.Counter}}.Count != 0)
                     {{indent}}{
                     {{indent}}    if (errors is null)
                     {{indent}}    {
                     {{indent}}        errors = new(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}    }
                     {{indent}}    else
                     {{indent}}    {
                     {{indent}}        errors.AddRange(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}    }
                     {{indent}}}
                     """;

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

    private string GetIfElseIfKeyword(RuleChainContext context)
    {
        return context.IfElseMode switch
        {
            IfElseMode.ElseIf => "else if",
            _ => "if"
        };
    }

    private string GetGotoLabelIfNeeded(string indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}        goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}