using System.Linq;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record IfRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    IfConditionModel IfCondition,
    EquatableArray<RuleChain> ChildRuleChains) : RuleChain(IsAsync, Object, null, Depth, Indent, NumberOfRules, null)
{
    public override bool NeedsGotoLabels()
    {
        return ChildRuleChains.Any(x => x.NeedsGotoLabels());
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (ChildRuleChains.Count == 0)
        {
            return string.Empty;
        }
        
        var code = $$"""
                   {{IfCondition.GenerateIfBlock(Object, GetRequestParameterName(), Indent, context)}}
                   {{Indent}}{
                   {{string.Join("\r\n", ChildRuleChains.Select(x => x.GenerateCode(context)))}}
                   {{Indent}}}
                   """;

        context.ResetIfElseMode();
        
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }
}