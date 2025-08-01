using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Extensions;

public static class StatementsParserExtensions
{
    public static LambdaInfo? GetLambdaInfoFromLastArgument(this InvocationExpressionSyntax invocation)
    {
        if (invocation.ArgumentList.Arguments.LastOrDefault()?.Expression is not LambdaExpressionSyntax lambda ||
            lambda.Body is not BlockSyntax blockSyntax)
        {
            return null;
        }

        var parameterName = lambda.GetParameterName();

        var statements = lambda.Body switch
        {
            BlockSyntax blockBody => blockBody.Statements.OfType<ExpressionStatementSyntax>(),
            _ => []
        };
        
        return new LambdaInfo(parameterName, statements);
    }
}