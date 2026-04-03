using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetWithRulesValidatorRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall) : DirectTargetRuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Indent, Object, Target!, context, 1);

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, Target.TargetPath.Value);

        ruleCodes.Add(GenerateValidatorCallCode(Indent, methodCall, context));
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
    }

}
