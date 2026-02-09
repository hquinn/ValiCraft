using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetValidateWithRuleChain(
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
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        string methodCall;
        if (IsAsync && IsAsyncValidatorCall)
        {
            methodCall = $"await {ValidatorExpression}.ValidateToListAsync({requestAccessor}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}.\", cancellationToken)";
        }
        else
        {
            methodCall = $"{ValidatorExpression}.ValidateToList({requestAccessor}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}.\")";
        }
        
        // Use a unique variable name suffix (counter) to avoid conflicts when multiple ValidateWith calls exist
        var code = $$"""
                     {{Indent}}var errors{{context.Counter}} = {{methodCall}};
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