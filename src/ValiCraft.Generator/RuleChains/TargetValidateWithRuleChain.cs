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
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        string methodCall;
        if (IsAsync && IsAsyncValidatorCall)
        {
            methodCall = $"await {ValidatorExpression}.RunValidationAsync({requestAccessor}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}.\", cancellationToken)";
        }
        else
        {
            methodCall = $"{ValidatorExpression}.RunValidation({requestAccessor}, $\"{{inheritedTargetPath}}{Target.TargetPath.Value}.\")";
        }

        // Use a unique variable name suffix (counter) to avoid conflicts when multiple ValidateWith calls exist
        // Always use 'if' here because the var declaration above breaks any if/else chain
        var code = $$"""
                     {{Indent}}var errors{{context.Counter}} = {{methodCall}};
                     {{Indent}}if (errors{{context.Counter}} is not null)
                     {{Indent}}{
                     {{Indent}}    if (errors is null)
                     {{Indent}}    {
                     {{Indent}}        errors = errors{{context.Counter}};
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