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
    IndentModel? ValidatorCallGotoIndent = null) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        // The var declaration breaks any if/else chain, so goto labels are required for halt semantics
        return true;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return BuildTargetPath(context.TargetPath, Config.Target!.TargetPath.Value, false, context.Counter);
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Config.Indent, Config.Object, Config.Target!, context, 1);

        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(Config.IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, context.TargetPath.TrimEnd('.'));

        ruleCodes.Add(GenerateValidatorCallCode(Config.Indent, methodCall, context, ValidatorCallGotoIndent));
        context.DecrementCountdown();
        context.UpdateIfElseMode();

        return string.Join("\r\n", ruleCodes);
    }

}
