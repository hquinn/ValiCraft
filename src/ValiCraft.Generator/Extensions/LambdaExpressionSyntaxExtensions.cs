using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Extensions;

public static class LambdaExpressionSyntaxExtensions
{
    public static string GetParameterName(this LambdaExpressionSyntax lambda)
    {
        return lambda switch
        {
            SimpleLambdaExpressionSyntax simpleLambda => simpleLambda.Parameter.Identifier.ValueText,
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.ParameterList.Parameters
                .First().Identifier.ValueText,
            _ => "_"
        };
    }

    /// <summary>
    /// Gets all parameter names from a lambda expression.
    /// For async lambdas with CancellationToken (e.g., async (x, ct) => ...), this returns both parameter names.
    /// </summary>
    public static IReadOnlyList<string> GetParameterNames(this LambdaExpressionSyntax lambda)
    {
        return lambda switch
        {
            SimpleLambdaExpressionSyntax simpleLambda => [simpleLambda.Parameter.Identifier.ValueText],
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.ParameterList.Parameters
                .Select(p => p.Identifier.ValueText)
                .ToList(),
            _ => ["_"]
        };
    }
}