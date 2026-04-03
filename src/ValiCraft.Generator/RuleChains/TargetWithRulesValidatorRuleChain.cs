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
    bool IsAsyncValidatorCall,
    CollectionConfig? Collection = null,
    bool HoistValidator = false) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
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

        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Indent, Object, Target!, context, 1);

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, Target.TargetPath.Value);

        ruleCodes.Add(GenerateValidatorCallCode(Indent, methodCall, context));
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
    }

    private string HandleCollectionCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var itemRequestName = GetItemRequestParameterName();
        var childIndent = IndentModel.CreateChild(Indent);

        // When hoisting, capture the validator variable name before rules decrement the counter
        var (loopCallTarget, hoistLine) = ResolveHoistTarget(HoistValidator, ValidatorCallTarget, Indent, context);

        // Create an object-level target for the item within the loop
        var itemTarget = CreateItemTarget(Collection!.ElementType, Target!);

        // Generate rule code for inside the loop
        var ruleCodes = GenerateRulesCode(Rules, itemRequestName, childIndent, Object, itemTarget, context, 1);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, loopCallTarget, itemRequestName, $"{Target!.TargetPath.Value}[{{{index}}}]");

        ruleCodes.Add(GenerateValidatorCallCode(childIndent, methodCall, context, IndentModel.CreateChild(childIndent)));
        context.DecrementCountdown();

        var code = GenerateForEachLoop(index, string.Join("\r\n", ruleCodes), hoistLine);

        // Reset because the foreach block breaks any if/else chain
        context.ResetIfElseMode();
        return code;
    }

}
