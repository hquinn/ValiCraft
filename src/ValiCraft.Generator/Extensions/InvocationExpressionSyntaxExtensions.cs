using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.Extensions;

public static class InvocationExpressionSyntaxExtensions
{
    public static IEnumerable<ArgumentInfo> GetArguments(
        this InvocationExpressionSyntax invocation,
        IMethodSymbol? methodSymbol,
        SemanticModel semanticModel)
    {
        // Your existing logic for getting arguments
        return invocation.ArgumentList.Arguments
            .Select((arg, i) =>
            {
                var argumentExpression = arg.Expression;
                var constantValueResult = semanticModel.GetConstantValue(argumentExpression);
                var type = semanticModel.GetTypeInfo(argumentExpression).Type;

                var name = methodSymbol?.Parameters[i].Name ?? "";
                var value = argumentExpression.ToString();
                var typeString = type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "";
                var typeInfo = new TypeInfo(typeString, type is not null && type.TypeKind == TypeKind.TypeParameter, type is not null && type.NullableAnnotation == NullableAnnotation.Annotated, argumentExpression is SimpleLambdaExpressionSyntax);
                var isLiteral = constantValueResult.HasValue;
                var constantValue = isLiteral ? constantValueResult.Value : null;
                
                return new ArgumentInfo(name, value, typeInfo, isLiteral, constantValue);
            });
    }
}