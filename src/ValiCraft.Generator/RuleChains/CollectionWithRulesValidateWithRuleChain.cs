using System.Collections.Generic;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains;

public record CollectionWithRulesValidateWithRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    TypeInfo ElementType,
    EquatableArray<Rule> Rules,
    string ValidatorExpression,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var validatorVar = $"validator{context.Counter}";
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();
        var childIndent = IndentModel.CreateChild(Indent);

        // Create an object-level target for the item within the loop
        var itemTarget = new ValidationTarget(
            AccessorType: AccessorType.Object,
            AccessorExpressionFormat: "{0}",
            Type: ElementType,
            DefaultTargetName: Target.DefaultTargetName,
            TargetPath: Target.TargetPath);

        // Generate rule code for inside the loop
        var ruleCodes = new List<string>(Rules.Count + 1);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                itemRequestName,
                childIndent,
                Object,
                itemTarget,
                context));
            context.DecrementCountdown();
        }

        // Generate the ValidateWith code inside the loop
        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, validatorVar, itemRequestName, $"{Target.TargetPath.Value}[{{{index}}}]");

        ruleCodes.Add(GenerateValidatorCallCode(childIndent, methodCall, context, IndentModel.CreateChild(childIndent)));
        context.DecrementCountdown();

        var ruleChainCodes = string.Join("\r\n", ruleCodes);

        var code = $$"""
                     {{Indent}}var {{validatorVar}} = {{ValidatorExpression}};
                     {{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{ruleChainCodes}}
                     {{Indent}}    {{index}}++;
                     {{Indent}}}
                     """;

        // Reset because the foreach block breaks any if/else chain
        context.ResetIfElseMode();
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}
