using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.IfConditions;

public record BlankIfConditionModel(bool IsRuleChainCondition) : IfConditionModel(IsRuleChainCondition)
{
    public override string GenerateIfBlock(
        ValidationTarget target,
        string requestName,
        IndentModel indent,
        RuleChainContext context)
    {
        if (IsRuleChainCondition)
        {
            return string.Empty;
        }

        return $"{indent}{context.GetIfElseIfKeyword()} (";
    }
}