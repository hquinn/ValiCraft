using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public abstract record DirectTargetRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
    }
}
