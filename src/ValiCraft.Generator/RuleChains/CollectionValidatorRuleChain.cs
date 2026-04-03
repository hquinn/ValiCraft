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
        var childIndent = IndentModel.CreateChild(Indent);

        // When hoisting, use a local variable for the validator to avoid repeated allocations in the loop
        var (loopCallTarget, hoistLine) = ResolveHoistTarget(context);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, loopCallTarget, GetItemRequestParameterName(), $"{Target!.TargetPath.Value}[{{{index}}}]");

        var validatorCallCode = GenerateValidatorCallCode(childIndent, methodCall, context);

        var code = GenerateForEachLoop(index, validatorCallCode, hoistLine);

        context.DecrementCountdown();
        // Reset because the foreach block breaks any if/else chain — the next chain cannot use 'else if'
        context.ResetIfElseMode();
        return code;
    }

    private (string CallTarget, string? HoistLine) ResolveHoistTarget(RuleChainContext context)
    {
        if (!HoistValidator)
        {
            return (ValidatorCallTarget, null);
        }

        var validatorVar = $"validator{context.Counter}";
        return (validatorVar, $"{Indent}var {validatorVar} = {ValidatorCallTarget};\r\n");
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}
