using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetWithRulesValidatorRuleChain(
    RuleChainConfig Config,
    EquatableArray<Rule> Rules,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall,
    CollectionConfig? Collection = null,
    bool HoistValidator = false) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return BuildTargetPath(context.TargetPath, Config.Target!.TargetPath.Value, Collection is not null, context.Counter, ".");
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (Collection is not null)
        {
            return HandleCollectionCodeGeneration(context);
        }

        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Config.Indent, Config.Object, Config.Target!, context, 1);

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(Config.IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, Config.Target.TargetPath.Value);

        ruleCodes.Add(GenerateValidatorCallCode(Config.Indent, methodCall, context));
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
    }

    private string HandleCollectionCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var itemRequestName = GetItemRequestParameterName();
        var childIndent = IndentModel.CreateChild(Config.Indent);

        // When hoisting, capture the validator variable name before rules decrement the counter
        var (loopCallTarget, hoistLine) = ResolveHoistTarget(HoistValidator, ValidatorCallTarget, Config.Indent, context);

        // Create an object-level target for the item within the loop
        var itemTarget = CreateItemTarget(Collection!.ElementType, Config.Target!);

        // Generate rule code for inside the loop
        var ruleCodes = GenerateRulesCode(Rules, itemRequestName, childIndent, Config.Object, itemTarget, context, 1);

        var methodCall = BuildValidatorMethodCall(Config.IsAsync, IsAsyncValidatorCall, loopCallTarget, itemRequestName, $"{Config.Target!.TargetPath.Value}[{{{index}}}]");

        ruleCodes.Add(GenerateValidatorCallCode(childIndent, methodCall, context, IndentModel.CreateChild(childIndent)));
        context.DecrementCountdown();

        var code = GenerateForEachLoop(index, string.Join("\r\n", ruleCodes), hoistLine);

        // Reset because the foreach block breaks any if/else chain
        context.ResetIfElseMode();
        return code;
    }

}
