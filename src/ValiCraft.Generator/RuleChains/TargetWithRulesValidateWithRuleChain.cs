using System.Collections.Generic;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetWithRulesValidateWithRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<Rule> Rules,
    string ValidatorExpression,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = new List<string>(Rules.Count + 1);

        // Generate code for each rule
        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                GetRequestParameterName(),
                Indent,
                Object,
                Target!,
                context));
            context.DecrementCountdown();
        }

        // Generate the ValidateWith code
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorExpression, requestAccessor, Target.TargetPath.Value);

        var validateCode = $$"""
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

        ruleCodes.Add(validateCode);
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
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
