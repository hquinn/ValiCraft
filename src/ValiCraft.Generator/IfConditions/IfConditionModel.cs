using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.IfConditions;

public enum IfConditionKind
{
    Blank,
    ExpressionFormat,
    BlockLambda
}

public sealed record IfConditionModel(
    IfConditionKind Kind,
    bool IsRuleChainCondition,
    string? ExpressionFormat = null,
    string? Body = null,
    string? Parameter = null)
{
    public static IfConditionModel Blank(bool isRuleChainCondition = false)
    {
        return new IfConditionModel(IfConditionKind.Blank, isRuleChainCondition);
    }

    public static IfConditionModel CreateExpressionFormat(string expressionFormat, bool isRuleChainCondition)
    {
        return new IfConditionModel(IfConditionKind.ExpressionFormat, isRuleChainCondition, ExpressionFormat: expressionFormat);
    }

    public static IfConditionModel CreateBlockLambda(string body, string parameter, bool isRuleChainCondition)
    {
        return new IfConditionModel(IfConditionKind.BlockLambda, isRuleChainCondition, Body: body, Parameter: parameter);
    }

    public string GenerateIfBlock(
        ValidationTarget target,
        string requestName,
        IndentModel indent,
        RuleChainContext context)
    {
        return Kind switch
        {
            IfConditionKind.Blank => GenerateBlank(indent, context),
            IfConditionKind.ExpressionFormat => GenerateExpressionFormat(target, requestName, indent, context),
            IfConditionKind.BlockLambda => GenerateBlockLambda(target, requestName, indent, context),
            _ => string.Empty
        };
    }

    private string GenerateBlank(IndentModel indent, RuleChainContext context)
    {
        if (IsRuleChainCondition)
        {
            return string.Empty;
        }

        return $"{indent}{context.GetIfElseIfKeyword()} (";
    }

    private string GenerateExpressionFormat(
        ValidationTarget target,
        string requestName,
        IndentModel indent,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);
        var inlinedCondition = string.Format(ExpressionFormat!, targetAccessor);

        if (IsRuleChainCondition)
        {
            return $$"""
                     {{indent}}if ({{inlinedCondition}})
                     """;
        }

        return $"{indent}{context.GetIfElseIfKeyword()} ({inlinedCondition} && ";
    }

    private string GenerateBlockLambda(
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

        return $"{localFunction}\r\n{indent}{context.GetIfElseIfKeyword()} ({inlinedCondition} && ";
    }
}
