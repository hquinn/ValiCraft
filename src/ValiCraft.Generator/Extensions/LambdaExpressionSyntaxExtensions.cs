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

    public static string? GetSecondParameterName(this LambdaExpressionSyntax lambda)
    {
        return lambda switch
        {
            ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: >= 2 } parenthesizedLambda =>
                parenthesizedLambda.ParameterList.Parameters[1].Identifier.ValueText,
            _ => null
        };
    }
}