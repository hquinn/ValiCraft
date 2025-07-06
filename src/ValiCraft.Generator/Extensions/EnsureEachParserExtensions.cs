using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Extensions;

public static class EnsureEachParserExtensions
{
    public static IEnumerable<ExpressionStatementSyntax> GetRuleStatementsFromEnsureEach(
        this InvocationExpressionSyntax ensureEachInvocation)
    {
        if (ensureEachInvocation.ArgumentList.Arguments.Count < 2 ||
            ensureEachInvocation.ArgumentList.Arguments[1].Expression is not LambdaExpressionSyntax lambda)
        {
            return [];
        }

        return lambda.Body switch
        {
            // The body of a lambda can be a block with curly braces or a direct expression.
            // If it's a block, we look inside its statements.
            BlockSyntax blockBody => blockBody.Statements.OfType<ExpressionStatementSyntax>(),
            // If it's a direct expression (e.g., item => item.Ensure(...)),
            // it will be an InvocationExpression, which is wrapped in an ExpressionStatement.
            ExpressionSyntax { Parent: ExpressionStatementSyntax statement } => [statement],
            _ => []
        };
    }
}