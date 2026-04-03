using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionValidatorRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall,
    bool HoistValidator) : RuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
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
        var childIndent = IndentModel.CreateChild(Indent);

        // When hoisting, use a local variable for the validator to avoid repeated allocations in the loop
        string loopCallTarget;
        string hoistLine;
        if (HoistValidator)
        {
            var validatorVar = $"validator{context.Counter}";
            loopCallTarget = validatorVar;
            hoistLine = $"{Indent}var {validatorVar} = {ValidatorCallTarget};\r\n";
        }
        else
        {
            loopCallTarget = ValidatorCallTarget;
            hoistLine = string.Empty;
        }

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, loopCallTarget, itemRequestName, $"{Target.TargetPath.Value}[{{{index}}}]");

        var validatorCallCode = GenerateValidatorCallCode(childIndent, methodCall, context);

        var code = $$"""
                     {{hoistLine}}{{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{validatorCallCode}}
                     {{Indent}}    {{index}}++;
                     {{Indent}}}
                     """;

        context.DecrementCountdown();
        // Reset because the foreach block breaks any if/else chain — the next chain cannot use 'else if'
        context.ResetIfElseMode();
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}
