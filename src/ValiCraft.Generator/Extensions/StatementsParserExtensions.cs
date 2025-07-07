using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Extensions;

public static class StatementsParserExtensions
{
    public static LambdaInfo? GetLambdaInfoFromLastArgument(this InvocationExpressionSyntax invocation)
    {
        if (invocation.ArgumentList.Arguments.LastOrDefault()?.Expression is not LambdaExpressionSyntax lambda)
        {
            return null;
        }
        
        var parameterName = lambda switch
        {
            // Handles simple lambdas: item => ...
            SimpleLambdaExpressionSyntax simpleLambda => simpleLambda.Parameter.Identifier.ValueText,
    
            // Handles parenthesized lambdas: (item) => ... or (item, index) => ...
            ParenthesizedLambdaExpressionSyntax parenLambda => parenLambda.ParameterList.Parameters.FirstOrDefault()?.Identifier.ValueText,
    
            // Default case for safety, though should not be hit with valid C#.
            _ => null
        };

        var statements = lambda.Body switch
        {
            // The body of a lambda can be a block with curly braces or a direct expression.
            // If it's a block, we look inside its statements.
            BlockSyntax blockBody => blockBody.Statements.OfType<ExpressionStatementSyntax>(),
            // If it's a direct expression (e.g., item => item.Ensure(...)),
            // it will be an InvocationExpression, which is wrapped in an ExpressionStatement.
            ExpressionSyntax { Parent: ExpressionStatementSyntax statement } => [statement],
            _ => []
        };
        
        return new LambdaInfo(parameterName, statements);
    }
}