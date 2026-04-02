using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetStaticValidateRuleChain(
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
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorTypeName, requestAccessor, Target.TargetPath.Value);

        // Use a unique variable name suffix (counter) to avoid conflicts when multiple Validate calls exist
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
