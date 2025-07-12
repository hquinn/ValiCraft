using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Concepts;

public record LambdaInfo(string? ParameterName, IEnumerable<ExpressionStatementSyntax> Statements)
{    
    public static bool IsValid(
        LambdaInfo? lambdaInfo,
        InvocationExpressionSyntax invocation,
        string ruleChain,
        List<DiagnosticInfo> diagnostics)
    {
        if (lambdaInfo is null)
        {
            diagnostics.Add(DefinedDiagnostics.InvalidLambdaDefined(ruleChain, invocation.GetLocation()));
            return false;
        }

        if (string.IsNullOrWhiteSpace(lambdaInfo.ParameterName))
        {
            diagnostics.Add(DefinedDiagnostics.MissingLambdaParameterName(invocation.GetLocation()));
            return false;
        }

        return true;
    }
}