using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains;

public record CollectionWithRulesValidatorRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    TypeInfo ElementType,
    EquatableArray<Rule> Rules,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall,
    bool HoistValidator) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
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

        // When hoisting, capture the validator variable name before rules decrement the counter
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

        // Create an object-level target for the item within the loop
        var itemTarget = CreateItemTarget(ElementType, Target!);

        // Generate rule code for inside the loop
        var ruleCodes = GenerateRulesCode(Rules, itemRequestName, childIndent, Object, itemTarget, context, 1);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, loopCallTarget, itemRequestName, $"{Target.TargetPath.Value}[{{{index}}}]");

        ruleCodes.Add(GenerateValidatorCallCode(childIndent, methodCall, context, IndentModel.CreateChild(childIndent)));
        context.DecrementCountdown();

        var ruleChainCodes = string.Join("\r\n", ruleCodes);

        var code = $$"""
                     {{hoistLine}}{{Indent}}var {{index}} = 0;
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
