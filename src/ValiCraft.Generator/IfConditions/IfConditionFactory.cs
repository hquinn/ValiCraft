using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.IfConditions;

public static class IfConditionFactory
{
    public static IfConditionModel? Create(InvocationExpressionSyntax invocation, bool isRuleChainCondition)
    {
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (argumentExpression is null)
        {
            return null;
        }

        switch (argumentExpression)
        {
            case LambdaExpressionSyntax { Body: IsPatternExpressionSyntax or BinaryExpressionSyntax } patternLambda:
                return CreatePatternLambda(patternLambda, isRuleChainCondition);
            case LambdaExpressionSyntax { Body: BlockSyntax } blockLambda:
                return CreateBlockLambda(blockLambda, isRuleChainCondition);
            case LambdaExpressionSyntax { Body: InvocationExpressionSyntax } invocationLambda:
                return CreateInvocationLambda(invocationLambda, isRuleChainCondition);
            case IdentifierNameSyntax identifierNameSyntax:
                return CreateIdentifierName(identifierNameSyntax, isRuleChainCondition);
        }

        return null;
    }

    private static IfConditionModel CreatePatternLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return IfConditionModel.CreateExpressionFormat(rewrittenBody.ToString(), isRuleChainCondition);
    }

    private static IfConditionModel CreateBlockLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();

        return IfConditionModel.CreateBlockLambda(lambda.Body.ToString(), parameterName, isRuleChainCondition);
    }

    private static IfConditionModel CreateInvocationLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return IfConditionModel.CreateExpressionFormat(rewrittenBody.ToString(), isRuleChainCondition);
    }

    private static IfConditionModel CreateIdentifierName(
        IdentifierNameSyntax identifierNameSyntax,
        bool isRuleChainCondition)
    {
        var expressionFormat = $"{identifierNameSyntax.Identifier.ValueText}({{0}})";

        return IfConditionModel.CreateExpressionFormat(expressionFormat, isRuleChainCondition);
    }
}
