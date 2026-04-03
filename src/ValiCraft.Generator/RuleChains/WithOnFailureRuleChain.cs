using System.Linq;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record WithOnFailureRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    EquatableArray<RuleChain> ChildRuleChains) : ContainerRuleChain(IsAsync, Object, Depth, Indent, NumberOfRules, FailureMode, ChildRuleChains)
{
    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var code = string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)));
        context.ResetIfElseMode();

        return code;
    }
}