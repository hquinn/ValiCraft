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
}