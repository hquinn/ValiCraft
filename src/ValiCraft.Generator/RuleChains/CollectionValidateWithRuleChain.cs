using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionValidateWithRuleChain(
    ValidationTarget Target,
    int Depth,
    OnFailureMode? FailureMode,
    string ValidatorExpression) : ValidateWithRuleChain(Target, Depth, FailureMode, ValidatorExpression)
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
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var indent = GetIndent();
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();
        
        // A little hacky with the assigned errors, but it's a quick fix to get uniqueness.
        var code = $$"""
                     {{indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{indent}}{
                     {{indent}}    var errors{{context.Counter}} = {{ValidatorExpression}}.ValidateToList({{itemRequestName}});
                     {{indent}}    if (errors{{context.Counter}}.Count != 0)
                     {{indent}}    {
                     {{indent}}        if (errors is null)
                     {{indent}}        {
                     {{indent}}            errors = new(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}        }
                     {{indent}}        else
                     {{indent}}        {
                     {{indent}}            errors.AddRange(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}        }
                     {{indent}}    }
                     {{indent}}}
                     """;

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

    private string GetGotoLabelIfNeeded(string indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}            goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }
}