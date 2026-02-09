using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.Extensions;

public static class InvocationExpressionSyntaxExtensions
{
    public static IEnumerable<ArgumentInfo> GetArguments(
        this InvocationExpressionSyntax invocation,
        IMethodSymbol? methodSymbol,
        SemanticModel semanticModel)
    {
        return invocation.ArgumentList.Arguments
            .Select((arg, i) =>
            {
                var argumentExpression = arg.Expression;
                var constantValueResult = semanticModel.GetConstantValue(argumentExpression);
                var type = semanticModel.GetTypeInfo(argumentExpression).Type;

                var name = methodSymbol?.Parameters[i].Name ?? "";
                var value = argumentExpression.ToString();
                var typeString = type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "";
                var typeInfo = new TypeInfo(typeString, type is not null && type.NullableAnnotation == NullableAnnotation.Annotated);
                var isLiteral = constantValueResult.HasValue;
                var constantValue = isLiteral ? constantValueResult.Value : null;
                
                return new ArgumentInfo(name, value, typeInfo, isLiteral, constantValue);
            });
    }
    
    public static IEnumerable<ArgumentInfo> GetRuleArguments(
        this InvocationExpressionSyntax invocation,
        SemanticModel semanticModel)
    {
        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0) return [];

        IMethodSymbol? ruleSymbol = null;
        IEnumerable<ArgumentSyntax> argumentsToProcess = [];

        var firstArgExpression = arguments[0].Expression;

        // SCENARIO 1: Lambda Expression 
        // e.g. .Is(x => Rules.GreaterThan(x, 0M))
        if (firstArgExpression is LambdaExpressionSyntax lambda)
        {
            // 1. Unwrap the body to find the inner invocation
            var innerInvocation = lambda.Body switch
            {
                InvocationExpressionSyntax i => i,
                AwaitExpressionSyntax { Expression: InvocationExpressionSyntax i } => i,
                _ => null
            };

            if (innerInvocation?.ArgumentList is not null)
            {
                // 2. Get the symbol from the INNER call (Rules.GreaterThan)
                ruleSymbol = semanticModel.GetSymbolInfo(innerInvocation).Symbol as IMethodSymbol;

                // 3. Process the inner arguments
                //    The inner call is "GreaterThan(x, 0M)". 
                //    Index 0 is the target 'x' (which matches the lambda parameter).
                //    We skip it because we only want the configuration arguments (0M).
                argumentsToProcess = innerInvocation.ArgumentList.Arguments.Skip(1);
            }
        }
        // SCENARIO 2: Method Group / Identifier
        // e.g. .Is(Rules.GreaterThan, 0M)
        else
        {
            // For Method Groups, we need at least 2 args: The Rule Name + The Value
            if (arguments.Count < 2) return [];

            // 1. Get the symbol from the FIRST argument (Rules.GreaterThan)
            ruleSymbol = semanticModel.GetSymbolInfo(firstArgExpression).Symbol as IMethodSymbol;

            // 2. Process the outer arguments
            //    We skip the first argument because it is the Rule Name itself.
            argumentsToProcess = arguments.Skip(1);
        }

        if (ruleSymbol is null)
        {
            return [];
        }

        // SHARED LOGIC: Map the extracted arguments to the rule's parameters
        return argumentsToProcess.Select((arg, i) =>
        {
            // We shift the index by +1 because the Rule's first parameter 
            // is always the object being validated (the "target"), which isn't in our list of values.
            // e.g. GreaterThan(target, limit) -> 'limit' is index 1.
            var paramIndex = i + 1;
            
            var name = (paramIndex < ruleSymbol.Parameters.Length)
                ? ruleSymbol.Parameters[paramIndex].Name 
                : ""; 

            var argumentExpression = arg.Expression;
            var constantValueResult = semanticModel.GetConstantValue(argumentExpression);
            var type = semanticModel.GetTypeInfo(argumentExpression).Type;
            
            var value = argumentExpression.ToString();
            var typeString = type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "";
            var typeInfo = new TypeInfo(typeString, type is not null && type.NullableAnnotation == NullableAnnotation.Annotated);
            var isLiteral = constantValueResult.HasValue;
            var constantValue = isLiteral ? constantValueResult.Value : null;

            return new ArgumentInfo(name, value, typeInfo, isLiteral, constantValue);
        });
    }
    
    public static EquatableArray<string> GetGenericArguments(this InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.Name is GenericNameSyntax genericName)
            {
                return genericName.TypeArgumentList.Arguments.Select(x => x.ToFullString()).ToEquatableImmutableArray();
            }
        }
    
        return EquatableArray<string>.Empty;
    }
}