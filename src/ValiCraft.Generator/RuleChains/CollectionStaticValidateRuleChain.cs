using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionStaticValidateRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorTypeName,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
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

        string methodCall;
        if (IsAsync && IsAsyncValidatorCall)
        {
            methodCall = $"await {ValidatorTypeName}.ValidateToListAsync({itemRequestName}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}[{{{index}}}].\", cancellationToken)";
        }
        else
        {
            methodCall = $"{ValidatorTypeName}.ValidateToList({itemRequestName}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}[{{{index}}}].\")";
        }
        
        // Use a unique variable name suffix (counter) to avoid conflicts when multiple Validate calls exist
        var code = $$"""
                     {{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{Indent}}    var errors{{context.Counter}} = {{methodCall}};
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
