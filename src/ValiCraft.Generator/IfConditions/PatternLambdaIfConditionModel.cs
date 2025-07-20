using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.IfConditions;

public record PatternLambdaIfConditionModel(string ExpressionFormat, bool IsRuleChainCondition)
    : IfConditionModel(IsRuleChainCondition)
{
    public override string GenerateIfBlock(
        ValidationTarget target,
        string requestName,
        IndentModel indent,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);
        var inlinedCondition = string.Format(ExpressionFormat, targetAccessor);

        if (IsRuleChainCondition)
        {
            return $$"""
                     {{indent}}if ({{inlinedCondition}})
                     """;
        }
        
        return $"{indent}{context.GetIfElseIfKeyword()} ({inlinedCondition} && ";
    }
}