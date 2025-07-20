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

    private static PatternLambdaIfConditionModel CreatePatternLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();
        
        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new PatternLambdaIfConditionModel(rewrittenBody.ToString(), isRuleChainCondition);
    }

    private static BlockLambdaIfConditionModel CreateBlockLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();
        
        return new BlockLambdaIfConditionModel(lambda.Body.ToString(), parameterName, isRuleChainCondition);
    }

    private static InvocationLambdaIfConditionModel CreateInvocationLambda(
        LambdaExpressionSyntax lambda,
        bool isRuleChainCondition)
    {
        var parameterName = lambda.GetParameterName();
        
        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new InvocationLambdaIfConditionModel(rewrittenBody.ToString(), isRuleChainCondition);
    }

    private static IdentifierNameIfConditionModel CreateIdentifierName(
        IdentifierNameSyntax identifierNameSyntax,
        bool isRuleChainCondition)
    {
        var expressionFormat = $"{identifierNameSyntax.Identifier.ValueText}({{0}})";
        
        return new IdentifierNameIfConditionModel(expressionFormat, isRuleChainCondition);
    }
}