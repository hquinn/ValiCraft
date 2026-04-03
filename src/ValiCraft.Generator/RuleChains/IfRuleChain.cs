using System.Linq;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record IfRuleChain(
    RuleChainConfig Config,
    IfConditionModel IfCondition,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        return ChildRuleChains.Any(x => x.NeedsGotoLabels());
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (ChildRuleChains.Count == 0)
        {
            return string.Empty;
        }

        var code = $$"""
                   {{IfCondition.GenerateIfBlock(Config.Object, GetRequestParameterName(), Config.Indent, context)}}
                   {{Config.Indent}}{
                   {{string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)))}}
                   {{Config.Indent}}}
                   """;

        context.ResetIfElseMode();

        return code;
    }
}