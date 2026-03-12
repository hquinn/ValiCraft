using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionValidateWithRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorExpression,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var validatorVar = $"validator{context.Counter}";
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();

        string methodCall;
        if (IsAsync && IsAsyncValidatorCall)
        {
            methodCall = $"await {validatorVar}.RunValidationAsync({itemRequestName}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}[{{{index}}}].\", cancellationToken)";
        }
        else
        {
            methodCall = $"{validatorVar}.RunValidation({itemRequestName}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}[{{{index}}}].\")";
        }

        // Use a unique variable name suffix (counter) to avoid conflicts when multiple ValidateWith calls exist
        // Hoist the validator expression before the loop to avoid repeated allocations
        var code = $$"""
                     {{Indent}}var {{validatorVar}} = {{ValidatorExpression}};
                     {{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{Indent}}    var errors{{context.Counter}} = {{methodCall}};
                     {{Indent}}    if (errors{{context.Counter}} is not null)
                     {{Indent}}    {
                     {{Indent}}        if (errors is null)
                     {{Indent}}        {
                     {{Indent}}            errors = errors{{context.Counter}};
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
        // Reset because the foreach block breaks any if/else chain — the next chain cannot use 'else if'
        context.ResetIfElseMode();
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