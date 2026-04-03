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
    bool HoistValidator) : CollectionItemRuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
    protected override string HandleCodeGeneration(RuleChainContext context)
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
