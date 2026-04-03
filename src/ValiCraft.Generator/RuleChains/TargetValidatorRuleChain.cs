using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetValidatorRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall,
    CollectionConfig? Collection = null,
    bool HoistValidator = false) : DirectTargetRuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        if (Collection is not null)
        {
            var indexer = $"index{context.Counter}";
            return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
        }

        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (Collection is not null)
        {
            return HandleCollectionCodeGeneration(context);
        }

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, Target.TargetPath.Value);

        // Always use 'if' here because the var declaration above breaks any if/else chain
        var code = GenerateValidatorCallCode(Indent, methodCall, context);

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

    private string HandleCollectionCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var childIndent = IndentModel.CreateChild(Indent);

        // When hoisting, use a local variable for the validator to avoid repeated allocations in the loop
        var (loopCallTarget, hoistLine) = ResolveHoistTarget(HoistValidator, ValidatorCallTarget, Indent, context);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, loopCallTarget, GetItemRequestParameterName(), $"{Target!.TargetPath.Value}[{{{index}}}]");

        var validatorCallCode = GenerateValidatorCallCode(childIndent, methodCall, context);

        var code = GenerateForEachLoop(index, validatorCallCode, hoistLine);

        context.DecrementCountdown();
        // Reset because the foreach block breaks any if/else chain — the next chain cannot use 'else if'
        context.ResetIfElseMode();
        return code;
    }

}
