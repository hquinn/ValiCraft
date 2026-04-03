using System.Collections.Generic;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetWithRulesStaticValidateRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules,
    string ValidatorTypeName,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Indent, Object, Target!, context, 1);

        // Generate the Validate<T> code
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorTypeName, requestAccessor, Target.TargetPath.Value);

        ruleCodes.Add(GenerateValidatorCallCode(Indent, methodCall, context));
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}
