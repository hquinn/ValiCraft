using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.IfConditions;

public record BlockLambdaIfConditionModel(string Body, string Parameter, bool IsRuleChainCondition)
    : IfConditionModel(IsRuleChainCondition)
{
    public override string GenerateIfBlock(
        ValidationTarget target,
        string requestName,
        IndentModel indent,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);
        
        var localFunctionName = IsRuleChainCondition ? $"__ifRuleChain_{context.Counter}" : $"__ifRule_{context.Counter}";
        var localFunction = $$"""
                            {{indent}}bool {{localFunctionName}}({{target.Type.FormattedTypeName}} {{Parameter}})
                            {{indent}}{{Body}}
                            """;
        
        var inlinedCondition = $"{localFunctionName}({targetAccessor})";

        if (IsRuleChainCondition)
        {
            return $$"""
                     {{localFunction}}
                     {{indent}}if ({{inlinedCondition}})
                     """;
        }

        return $$"""
                 {{localFunction}}
                 {{indent}}{{context.GetIfElseIfKeyword()}} ({{inlinedCondition}} && 
                 """;
    }
}