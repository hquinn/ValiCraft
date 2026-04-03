using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record TargetValidatorRuleChain(
    RuleChainConfig Config,
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall) : RuleChain(Config)
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
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);

        var methodCall = BuildValidatorMethodCall(Config.IsAsync, IsAsyncValidatorCall, ValidatorCallTarget, requestAccessor, context.TargetPath.TrimEnd('.'));

        // Always use 'if' here because the var declaration above breaks any if/else chain
        var code = GenerateValidatorCallCode(Config.Indent, methodCall, context);

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

}
