using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetValidateWithRuleChain(
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorExpression) : RuleChain(Object, Target, Depth, Indent, 1, FailureMode)
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
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        
        // A little hacky with the assigned errors, but it's a quick fix to get uniqueness.
        var code = $$"""
                     {{Indent}}var errors{{context.Counter}} = {{ValidatorExpression}}.ValidateToList({{requestAccessor}}, $"{inheritedTargetPath}{{Target.TargetPath.Value}}.");
                     {{Indent}}{{context.GetIfElseIfKeyword()}} (errors{{context.Counter}}.Count != 0)
                     {{Indent}}{
                     {{Indent}}    if (errors is null)
                     {{Indent}}    {
                     {{Indent}}        errors = new(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(context)}}{{Indent}}    }
                     {{Indent}}    else
                     {{Indent}}    {
                     {{Indent}}        errors.AddRange(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(context)}}{{Indent}}    }
                     {{Indent}}}
                     """;

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

    private string GetGotoLabelIfNeeded(RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {Indent}        goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}