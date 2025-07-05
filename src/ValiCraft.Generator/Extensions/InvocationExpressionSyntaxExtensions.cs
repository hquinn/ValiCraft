using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Extensions;

public static class InvocationExpressionSyntaxExtensions
{
    public static IEnumerable<ArgumentInfo> GetArguments(
        this InvocationExpressionSyntax invocation,
        IMethodSymbol? methodSymbol,
        SemanticModel semanticModel,
        IEnumerable<ArgumentInfo> prependWith)
    {
        // Your existing logic for getting arguments
        return prependWith.Concat(invocation.ArgumentList.Arguments
            .Select((arg, i) =>
            {
                string name;

                if (methodSymbol is null)
                {
                    name = "";
                }
                else
                {
                    name = methodSymbol.Parameters[i].Name;
                }

                var value = arg.Expression.ToString();
                var type = semanticModel.GetTypeInfo(arg.Expression).Type;
                var isLiteral = arg.Expression is LiteralExpressionSyntax;

                return type is not null
                    ? new ArgumentInfo(name, value, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        isLiteral)
                    : new ArgumentInfo(name, value, "ERROR", isLiteral);
            }));
    }
}