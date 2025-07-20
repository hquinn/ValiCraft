using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionValidateWithRuleChain(
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
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();
        
        // A little hacky with the assigned errors, but it's a quick fix to get uniqueness.
        var code = $$"""
                     {{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{Indent}}    var errors{{context.Counter}} = {{ValidatorExpression}}.ValidateToList({{itemRequestName}}, $"{inheritedTargetPath}{{Target.TargetPath.Value}}[{{{index}}}].");
                     {{Indent}}    if (errors{{context.Counter}}.Count != 0)
                     {{Indent}}    {
                     {{Indent}}        if (errors is null)
                     {{Indent}}        {
                     {{Indent}}            errors = new(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(context)}}{{Indent}}        }
                     {{Indent}}        else
                     {{Indent}}        {
                     {{Indent}}            errors.AddRange(errors{{context.Counter}});
                     {{GetGotoLabelIfNeeded(context)}}{{Indent}}        }
                     {{Indent}}    }
                     {{Indent}}    {{index}}++;
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
                    {Indent}            goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}